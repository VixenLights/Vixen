#nullable enable

using Microsoft.VisualBasic;
using System.ComponentModel;
using System.Drawing;
using Vixen.Attributes;
using Vixen.Services;
using Vixen.Sys;
using Vixen.Sys.Props;
using VixenModules.Property.Color;

namespace VixenModules.App.Props.Models
{
	public abstract class BaseLightProp : BaseProp
	{
		private StringTypes _stringType;
		protected bool UpdateInProgress = false;

		protected BaseLightProp(string name, PropType propType) : base(name, propType)
		{
			StringType = StringTypes.Pixel;
		}

		[PropertyOrder(1)]
		[DisplayName("String Type")]
		public StringTypes StringType
		{
			get => _stringType;
			set => SetProperty(ref _stringType, value);
		}

		/// <summary>
		/// Adds a number of strings to the element tree
		/// </summary>
		/// <param name="node"></param>
		/// <param name="count"></param>
		/// <param name="nodesPerString"></param>
		/// <param name="namingIndex"></param>
		protected void AddStringElements(ElementNode node, int count, int nodesPerString, int namingIndex = 0)
		{
			if (count == 0) return;
			for (int i = namingIndex; i < count + namingIndex; i++)
			{
				string stringName = $"{AutoPropStringName} {i + 1}";
				ElementNode stringNode = ElementNodeService.Instance.CreateSingle(node, stringName, true, false);

				if (StringType == StringTypes.Pixel)
				{
					AddNodeElements(stringNode, nodesPerString);
				}
			}
		}

		protected async Task UpdateNodesPerString(int nodesPerString)
		{
			while (UpdateInProgress)
			{
				await Task.Delay(500);
			}
			UpdateInProgress = true;

			var propNode = GetOrCreatePropElementNode();

			foreach (var propString in propNode.Children.ToList())
			{
				var existingNodes = propString.Children.Count();
				if (existingNodes == nodesPerString) continue;

				if (existingNodes < nodesPerString)
				{
					var newNodes = AddNodeElements(propString, nodesPerString - existingNodes, existingNodes);
					await PropertySetupHelper.AddOrUpdateColorHandling(newNodes, GetColorConfiguration());
				}
				else
				{
					RemoveElements(propString, existingNodes - nodesPerString);
				}
			}

			UpdateInProgress = false;

		}

		protected async Task UpdateStrings(int strings)
		{
			while (UpdateInProgress)
			{
				await Task.Delay(500);
			}
			UpdateInProgress = true;
			var propNode = GetOrCreatePropElementNode();
			var existingStrings = propNode.Children.Count();
			if (existingStrings == strings) return;

			if (existingStrings < strings)
			{
				AddStringElements(propNode, strings - existingStrings, existingStrings, existingStrings);
			}
			else
			{
				RemoveElements(propNode, existingStrings - strings);
			}

			UpdateInProgress = false;
		}

		protected ColorConfiguration GetColorConfiguration()
		{
			// TODO Get this info from the Tree properties at some point
			ColorConfiguration cf = new()
			{
				ColorType = StringType == StringTypes.Pixel
					? ElementColorType.FullColor
					: ElementColorType.SingleColor,
				FullColorOrder = StringType == StringTypes.Pixel ? "RGB" : String.Empty,
				SingleColor = StringType == StringTypes.Standard ? Color.Red : Color.Empty
			};
			return cf;
		}
		
	}
}
