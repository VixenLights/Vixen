
#nullable enable
using Common.Controls.ColorManagement.ColorModels;
using Common.Controls.Theme;
using Common.WPFCommon.Converters;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Vixen.Extensions;
using Vixen.Services;
using Vixen.Sys.Managers;
using Vixen.Sys.Props;
using Vixen.Sys.Props.Components;
using VixenModules.Property.Color;

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
	public class ArchProp : BaseLightProp<ArchModel>, IProp
	{
		#region Constructors

		public ArchProp() : this("Arch Temp")
		{
		}

		public ArchProp(string name, int nodeCount = 0, StringTypes stringType = StringTypes.ColorMixingRGB) : base(name, PropType.Arch)
		{			
			// Set some default parameters
			Name = name;
			NodeCount = 24;
			LightSize = 2;
			ArchWiringStart = ArchStartLocation.Left;
			LeftRight = true;

			Brightness = 100;
			Gamma = 1;
			Curve = null;

			StringType = stringType;
			var staticData = ApplicationServices.GetModuleStaticData(ColorDescriptor.ModuleId) as ColorStaticData;
			if (staticData != null)
			{
				var ColorSetNames = new ObservableCollection<string>(staticData.GetColorSetNames());
				SelectedColorSet = ColorSetNames[0];
			}						
		}

		#endregion

		#region Public Properties

		public int NodeCount
		{
			get => PropModel.NumPoints;
			set
			{
				if (value == PropModel.NumPoints)
				{
					return;
				}

				PropModel.NumPoints = value;
				OnPropertyChanged(nameof(NodeCount));
			}
		}

		private ArchStartLocation _archWiringStart;
		public ArchStartLocation ArchWiringStart
		{
			get => _archWiringStart;
			set
			{
				_archWiringStart = value;
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
				UpdateDefaultPropComponents();
			}
		}

		#endregion
		
		#region Public Method Overrides

		/// <summary>
		/// Get the HTML summary of all the parameter values
		/// </summary>
		/// <returns>Returns the <see cref="string"/> summary in HTML format</returns>
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
				$"<b>Starting Position:</b> {EnumValueTypeConverter.GetDescription(ArchWiringStart)}<br>" +
				$"<b>{AxisRotations[0].Axis} Rotation:</b> {AxisRotations[0].RotationAngle}\u00B0<br>" +
				$"<b>{AxisRotations[1].Axis} Rotation:</b> {AxisRotations[1].RotationAngle}\u00B0<br>" +
				$"<b>{AxisRotations[2].Axis} Rotation:</b> {AxisRotations[2].RotationAngle}\u00B0<br>" +
				 "</body>" +
				 "<h2>Additional Props</h2>" +
				 "<body>" +
				$"<b>Left and Right Arches:</b> {LeftRight}<br>" +
				 "</body>" +
				GetDimmingSummary() +
				GetColorSummary() +
				GetBaseSummary();

			return Summary;
		}

		#endregion

		#region Protected Methods

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		protected override async Task GenerateElementsAsync()
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

		protected override async void Prop_PropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
            try
            {
				if (e.PropertyName != null)
				{
					// Call base class implementation
					base.Prop_PropertyChanged(sender, e);

					switch (e.PropertyName)
					{
						case nameof(StringType):
							GenerateDebouncer.Debounce();
							break;
						case nameof(NodeCount):							
							GenerateDebouncer.Debounce();
							break;
						case nameof(ArchWiringStart):
                            await UpdatePatchingOrder();
							break;						
					}
				}
            }
            catch (Exception ex)
            {
				Logging.Error(ex, $"An error occured handling Arch property {e.PropertyName} changed");
			}
		}
		
		#endregion

		#region Private Methods

		private async Task UpdatePatchingOrder()
		{
			await AddOrUpdatePatchingOrder(ArchWiringStart == ArchStartLocation.Left ? Props.StartLocation.BottomLeft : Props.StartLocation.BottomRight);
		}

		/// <summary>
		/// Update the nodes for the Arch Prop
		/// </summary>
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

			// Is LeftRight specified, then create two additional props
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


				// Figure what the middle node number is and create a left from 0 to middle, and a right from middle to last
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

			// Else, no LeftRight is specified, so remove these
			else
			{
				// Remove Right
				if (PropComponents.Count == 3)
				{
					PropComponents.Remove(PropComponents[2]);
				}

				// Remove Left
				if (PropComponents.Count == 2)
				{
					PropComponents.Remove(PropComponents[1]);
				}
			}
		}
		#endregion		
	}
}