#nullable enable

using NLog;
using System.ComponentModel;
using System.Drawing;
using System.Reflection.Emit;
using Vixen.Attributes;
using Vixen.Services;
using Vixen.Sys;
using Vixen.Sys.Props;
using VixenModules.Property.Color;

namespace VixenModules.App.Props.Models
{
	public abstract class BaseLightProp : BaseProp
	{
		private static readonly Logger Logging = LogManager.GetCurrentClassLogger();
		
		private StringTypes _stringType;
		protected bool UpdateInProgress = false;

		protected BaseLightProp(string name, PropType propType) : base(name, propType)
		{
			StringType = StringTypes.Pixel;
		}

		/// <summary>
		/// Gets or sets the type of string used in the light prop.
		/// </summary>
		/// <value>
		/// A value of the <see cref="StringTypes"/> enumeration that specifies the string type.
		/// </value>
		/// <remarks>
		/// The <see cref="StringType"/> property determines the behavior and configuration of the light prop:
		/// <list type="bullet">
		/// <item>
		/// <description><see cref="StringTypes.Pixel"/> indicates that the string supports full-color pixels.</description>
		/// </item>
		/// <item>
		/// <description><see cref="StringTypes.Standard"/> indicates that the string supports single-color lights.</description>
		/// </item>
		/// </list>
		/// </remarks>
		[PropertyOrder(2)]
		[DisplayName("String Type")]
		public StringTypes StringType
		{
			get => _stringType;
			set => SetProperty(ref _stringType, value);
		}

		/// <summary>
		/// Adds a specified number of string elements to the given element node.
		/// </summary>
		/// <param name="node">The parent <see cref="ElementNode"/> to which the string elements will be added.</param>
		/// <param name="count">The number of string elements to add.</param>
		/// <param name="nodesPerString">The number of nodes to include in each string element.</param>
		/// <param name="namingIndex">
		/// The starting index for naming the string elements. Defaults to 0.
		/// Each string element will be named using the format "<c>AutoPropStringName X</c>",
		/// where <c>X</c> is the index starting from <paramref name="namingIndex"/>.
		/// </param>
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

		/// <summary>
		/// Updates the number of child nodes for the specified prop node to match the given count.
		/// </summary>
		/// <param name="nodeCount">The desired number of child nodes for the prop.</param>
		/// <param name="propNode">
		/// The <see cref="ElementNode"/> representing the prop whose child nodes are to be updated. 
		/// If <c>null</c>, the method retrieves or creates the prop's element node.
		/// </param>
		/// <exception cref="InvalidOperationException">
		/// Thrown if the specified <paramref name="propNode"/> is a leaf node or has a maximum child depth greater than 1.
		/// </exception>
		/// <remarks>
		/// This method adjusts the number of child nodes by either adding or removing nodes to match the specified 
		/// <paramref name="nodeCount"/>. It ensures that color handling is updated for the new nodes. If the operation 
		/// is already in progress, it waits until the current update completes.
		/// </remarks>
		/// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
		protected async Task UpdateStringNodeCount(int nodeCount, ElementNode? propNode = null)
		{
			propNode ??= GetOrCreatePropElementNode();
			
			if (propNode.IsLeaf || propNode.GetMaxChildDepth() > 2)
			{
				throw new InvalidOperationException("Prop does not have direct leaf children");
			}
			
			while (UpdateInProgress)
			{
				await Task.Delay(500);
			}
			UpdateInProgress = true;
			
			var existingNodes = propNode.Children.Count();
			if (existingNodes == nodeCount) return;

			if (existingNodes < nodeCount)
			{
				var newNodes = AddNodeElements(propNode, nodeCount - existingNodes, existingNodes);
				await PropertySetupHelper.AddOrUpdateColorHandling(newNodes, GetColorConfiguration());
			}
			else
			{
				RemoveElements(propNode, existingNodes - nodeCount);
			}

			UpdateInProgress = false;
		}

		/// <summary>
		/// Updates the number of nodes for each string element under the specified parent node.
		/// Ensures that the structure of the parent node adheres to the expected hierarchy: 
		/// <c>propNode -> Child Group(s) -> Leafs</c>.
		/// </summary>
		/// <param name="nodesPerString">
		/// The desired number of nodes for each string element under the parent node.
		/// </param>
		/// <param name="propNode">
		/// The parent <see cref="ElementNode"/> representing the prop whose child nodes are to be updated.
		/// If <c>null</c>, the method retrieves or creates the default prop element node.
		/// </param>
		/// <returns>
		/// A <see cref="Task"/> representing the asynchronous operation.
		/// </returns>
		/// <exception cref="InvalidOperationException">
		/// Thrown if the <paramref name="propNode"/> is a leaf node or its maximum child depth exceeds 2.
		/// </exception>
		protected async Task UpdateNodesPerString(int nodesPerString, ElementNode? propNode = null)
		{
			propNode ??= GetOrCreatePropElementNode();
			
			if (propNode.IsLeaf || propNode.GetMaxChildDepth() > 3)
			{
				throw new InvalidOperationException("Prop does not have direct leaf children");
			}
			
			while (UpdateInProgress)
			{
				await Task.Delay(500);
			}
			UpdateInProgress = true;

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

		/// <summary>
		/// Updates the number of string elements in the specified <see cref="ElementNode"/> to match the desired count.
		/// </summary>
		/// <param name="strings">
		/// The desired number of string elements to be present in the <paramref name="propNode"/>.
		/// </param>
		/// <param name="propNode">
		/// The <see cref="ElementNode"/> representing the parent node whose child string elements are to be updated.
		/// If <c>null</c>, the method will retrieve or create the default prop element node.
		/// </param>
		/// <remarks>
		/// This method ensures that the number of child string elements in the <paramref name="propNode"/> matches the specified count.
		/// If the current count is less than the desired count, new string elements are added. If the count exceeds the desired count,
		/// excess string elements are removed. The method uses the <see cref="UpdateInProgress"/> flag to ensure sequential updates
		/// and avoid conflicts.
		/// </remarks>
		/// <returns>
		/// A <see cref="Task"/> representing the asynchronous operation.
		/// </returns>
		protected async Task UpdateStrings(int strings, ElementNode? propNode = null)
		{
			propNode ??= GetOrCreatePropElementNode();
			
			while (UpdateInProgress)
			{
				await Task.Delay(500);
			}
			UpdateInProgress = true;
			
			var existingStrings = propNode.Children.Count();
			if (existingStrings == strings) return;

			if (existingStrings < strings)
			{
				var nodesPerString = existingStrings > 0 ? propNode.Children.First().Count() : 0;
				AddStringElements(propNode, strings - existingStrings, nodesPerString, existingStrings);
			}
			else
			{
				RemoveElements(propNode, existingStrings - strings);
			}

			UpdateInProgress = false;
		}

		/// <summary>
		/// Retrieves the color configuration for the current light prop.
		/// </summary>
		/// <returns>
		/// A <see cref="ColorConfiguration"/> object that specifies the color type, 
		/// full color order, and single color based on the <see cref="StringType"/> of the prop.
		/// </returns>
		/// <remarks>
		/// The color configuration is determined by the <see cref="StringType"/> property:
		/// <list type="bullet">
		/// <item>
		/// <description>If the <see cref="StringType"/> is <see cref="StringTypes.Pixel"/>, the color type is set to <see cref="ElementColorType.FullColor"/>, and the full color order is "RGB".</description>
		/// </item>
		/// <item>
		/// <description>If the <see cref="StringType"/> is <see cref="StringTypes.Standard"/>, the color type is set to <see cref="ElementColorType.SingleColor"/>, and the single color is set to red.</description>
		/// </item>
		/// </list>
		/// </remarks>
		protected ColorConfiguration GetColorConfiguration()
		{
			// TODO Get color order info from the Prop properties at some point
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

		/// <summary>
		/// Adds or updates the patching order for the current prop based on the specified parameters.
		/// </summary>
		/// <param name="startLocation">
		/// The starting location for the patching order. This determines where the patching begins.
		/// </param>
		/// <param name="zigZag">
		/// A boolean value indicating whether the patching order should follow a zig-zag pattern.
		/// </param>
		/// <param name="zigZagOffset">
		/// The offset value for the zig-zag pattern. This specifies how many elements are skipped before reversing the direction.
		/// </param>
		/// <remarks>
		/// This method ensures that the patching order is updated in a thread-safe manner by preventing concurrent updates.
		/// It utilizes the <see cref="PropertySetupHelper.AddOrUpdatePatchingOrder"/> method to apply the patching order.
		/// </remarks>
		/// <returns>
		/// A task representing the asynchronous operation.
		/// </returns>
		protected virtual async Task AddOrUpdatePatchingOrder(StartLocation startLocation = StartLocation.BottomLeft, bool zigZag = false,
			int zigZagOffset = 0)

		{
			while (UpdateInProgress)
			{
				await Task.Delay(500);
			}

			UpdateInProgress = true;
			var propNode = GetOrCreatePropElementNode();
			PropertySetupHelper.AddOrUpdatePatchingOrder(propNode, startLocation, zigZag, zigZagOffset);
			
			UpdateInProgress = false;
		}

		/// <summary>
		/// Asynchronously adds or updates the color handling configuration for the specified element node.
		/// </summary>
		/// <param name="node">
		/// The <see cref="IElementNode"/> to which the color handling configuration should be applied. 
		/// If <c>null</c>, the method will retrieve or create the default property element node.
		/// </param>
		/// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
		/// <remarks>
		/// This method ensures that only one update operation is performed at a time by utilizing a locking mechanism.
		/// If an exception occurs during the update process, it is logged for debugging purposes.
		/// </remarks>
		protected async Task AddOrUpdateColorHandling(IElementNode? node = null)
		{
			while (UpdateInProgress)
			{
				await Task.Delay(500);
			}

			UpdateInProgress = true;
			try
			{
				var propNode = node ?? GetOrCreatePropElementNode();
				await PropertySetupHelper.AddOrUpdateColorHandling(propNode, GetColorConfiguration());
			}
			catch (Exception e)
			{
				Logging.Error(e, "Error occured updating the color handling");
			}

			UpdateInProgress = false;
		}

	}
}
