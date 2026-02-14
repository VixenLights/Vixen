
#nullable enable
using AsyncAwaitBestPractices;
using Common.Controls.Theme;
using Debounce.Core;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using Common.Controls.ColorManagement.ColorModels;
using Vixen.Attributes;
using Vixen.Extensions;
using Vixen.Sys;
using Vixen.Sys.Managers;
using Vixen.Sys.Props;
using Vixen.Sys.Props.Components;
using Vixen.Sys.Props.Model;
using VixenModules.App.Curves;
using VixenModules.App.Props.Models;

namespace VixenModules.App.Props.Models.Arch
{
	public enum ArchStartLocation
	{
		Left,
		Right
	}

	/// <summary>
	/// A class that defines an Arch Prop
	/// </summary>
	public class Arch : BaseLightProp<ArchModel>, IProp
	{
		private readonly Debouncer _generateDebouncer;

		public Arch() : this("Arch Temp")
		{
		}

		public Arch(string name, int nodeCount = 0, StringTypes stringType = StringTypes.Pixel) : base(name, PropType.Arch)
		{
			Name = name;
			StringType = stringType;
			NodeCount = 29;
			LightSize = 4;
			StringType = StringTypes.Pixel;
			ArchWiringStart = ArchStartLocation.Left;
			LeftRight = false;

			DimmingTypeOption = DimmingType.NoCurve;
			Brightness = 80;
			Gamma = 2.2;
			Curve = null;

			PropertyChanged += Arch_PropertyChanged;

			// Create Preview model
//			PropModel = new ArchModel();

			_generateDebouncer = new Debouncer(() =>
			{
				GenerateElementsAsync().SafeFireAndForget();
			}, 500);
		}

		private void Arch_PropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			try
			{
				if (e.PropertyName != null)
				{
					switch (e.PropertyName)
					{
					}
				}
			}
			catch (Exception ex)
			{
				Logging.Error(ex, $"An error occured handling Arch property {e.PropertyName} changed");
			}

		}

		private int _nodeCount;
		public int NodeCount
		{
			get => _nodeCount;
			set
			{
				_nodeCount = value;
				_generateDebouncer?.Debounce();
//ToDo(1)				PropModel?.UpdatePropNodes();
				OnPropertyChanged(nameof(NodeCount));
			}
		}

		private int _lightSize;
		public int LightSize
		{
			get => _lightSize;
			set
			{
				_lightSize = value;
//ToDo(1)				PropModel?.UpdatePropNodes();
				OnPropertyChanged(nameof(LightSize));
			}
		}

		private ArchStartLocation _archWiringStart;
		public ArchStartLocation ArchWiringStart
		{
			get => _archWiringStart;
			set
			{
				_archWiringStart = value;
				UpdatePatchingOrder().SafeFireAndForget(); ;
				OnPropertyChanged(nameof(ArchWiringStart));
			}
		}

		private bool _leftRight;
		public bool LeftRight
		{
			get => _leftRight;
			set
			{
				_leftRight = value;
				OnPropertyChanged(nameof(LeftRight));
			}
		}

		public override string GetSummary()
		{
			string Summary =
				"<style>" +
				$"  h2   {{color: #{new RGB(ThemeColorTable.ForeColor).ToHex()}; margin-top: 0; margin-bottom: 0; text-decoration: underline;}}" +
				$"  body {{color: #{new RGB(ThemeColorTable.ForeColor).ToHex()}; margin-top: 0;}}" +
				$"  b    {{color: #{new RGB(ThemeColorTable.ForeColorDisabled).ToHex()}; margin-left: 20px;}}" +
				"</style>" +
				$"<h2>Basic Attributes</h2>" +
				"<body>" +
				$"<b>Prop Type:</b> {PropType.Arch.GetEnumDescription()}<br>" +
				$"<b>Name:</b> {Name}<br>" +
				$"<b>Light Count:</b> {NodeCount}<br>" +
				$"<b>Light Size:</b> {LightSize}<br>" +
				$"<b>Light Type:</b> {StringType}<br>" +
				$"<b>Starting Position:</b> {ArchWiringStart}<br>" +
				$"<b>{Rotations[0].Axis} Rotation:</b> {Rotations[0].RotationAngle}\u00B0<br>" +
				$"<b>{Rotations[1].Axis} Rotation:</b> {Rotations[1].RotationAngle}\u00B0<br>" +
				$"<b>{Rotations[2].Axis} Rotation:</b> {Rotations[2].RotationAngle}\u00B0<br>" +
				"</body>" +
				GetDimmingSummary();

			return Summary;
		}

		protected async Task GenerateElementsAsync()
		{
			await Task.Factory.StartNew(async () =>
			{
				bool hasUpdated = false;

				var propNode = GetOrCreateElementNode();
				if (propNode.IsLeaf)
				{
					AddNodeElements(propNode, NodeCount);
					hasUpdated = true;
				}
				else if (propNode.Children.Count() != NodeCount)
				{
					await UpdateStringNodeCount(NodeCount);
					hasUpdated = true;
				}

				if (hasUpdated)
				{
					await UpdatePatchingOrder();

					await AddOrUpdateColorHandling();

					UpdateDefaultPropComponents();
				}

				return true;

			});
		}

		private async Task UpdatePatchingOrder()
		{
			await AddOrUpdatePatchingOrder(ArchWiringStart == ArchStartLocation.Left ? Props.StartLocation.BottomLeft : Props.StartLocation.BottomRight);
		}

		#region PropComponents

		private void UpdateDefaultPropComponents()
		{
			var head = GetOrCreateElementNode();
			if (!PropComponents.Any())
			{
				var propComponent = PropComponentManager.CreatePropComponent($"{Name} Nodes", Id, PropComponentType.PropDefined);
				foreach (var stringNode in head.Children)
				{
					propComponent.TryAdd(stringNode);
				}
				PropComponents.Add(propComponent);
			}

			//Add new nodes, if any
			else if (head.Count() > PropComponents.First().TargetNodes.Count())
			{
				foreach (var node in head.Children.Except(PropComponents.First().TargetNodes))
				{
					PropComponents.First().TryAdd(node);
				}
			}
			else
			{
				//Remove nodes that no longer exist, if any
				foreach (var node in PropComponents.First().TargetNodes.Except(head.Children).ToList())
				{
					PropComponents.First().TryRemove(node.Id, out _);
				}
			}

			if (LeftRight == true)
			{
				//Update the left and right to match the new node count
				var propComponentLeft = PropComponents.FirstOrDefault(x => x.Name == $"{Name} Left");
				var propComponentRight = PropComponents.FirstOrDefault(x => x.Name == $"{Name} Right");

				if (propComponentLeft == null)
				{
					propComponentLeft = PropComponentManager.CreatePropComponent($"{Name} Left", Id, PropComponentType.PropDefined);
					PropComponents.Add(propComponentLeft);
				}
				else
				{
					propComponentLeft.Clear();
				}

				if (propComponentRight == null)
				{
					propComponentRight = PropComponentManager.CreatePropComponent($"{Name} Right", Id, PropComponentType.PropDefined);
					PropComponents.Add(propComponentRight);
				}
				else
				{
					propComponentRight.Clear();
				}

				int middle = (int)Math.Round(NodeCount / 2d, MidpointRounding.AwayFromZero);
				int i = 0;
				foreach (var stringNode in head.Children)
				{
					if (i < middle)
					{
						propComponentLeft.TryAdd(stringNode);
					}
					else
					{
						propComponentRight.TryAdd(stringNode);
					}

					i++;
				}
			}
		}
		#endregion
	}
}