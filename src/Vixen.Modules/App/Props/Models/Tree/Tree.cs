#nullable enable

using Common.Controls.ColorManagement.ColorModels;
using Common.Controls.Theme;
using Common.WPFCommon.Converters;
using System.ComponentModel;
using Vixen.Extensions;
using Vixen.Sys;
using Vixen.Sys.Managers;
using Vixen.Sys.Props;
using Vixen.Sys.Props.Components;

namespace VixenModules.App.Props.Models.Tree
{
	/// <summary>
	/// Maintains a tree prop.
	/// </summary>
	public class Tree : BaseLightProp<TreeModel>, IProp
	{		
		#region Constructors

		public Tree() : this("Tree 1", 0, 0)
		{
			//Set initial to 0 so creation does not trigger element generation.
		}

		public Tree(string name, int strings, int nodesPerString) : this(name, strings, nodesPerString, StringTypes.ColorMixingRGB)
		{
		}

		public Tree(string name, int strings = 0, int nodesPerString = 0, StringTypes stringType = StringTypes.ColorMixingRGB) : base(name, PropType.Tree)
		{			
			PropType = PropType.Tree;
			Name = name;
			StringType = stringType;
			ZigZagOffset = 50;
			StartLocation = StartLocation.BottomLeft;
			TopWidth = 20;
			TopHeight = TopWidth / 2;
			BaseHeight = 40;
			DegreesCoverage = 360;
			DegreeOffset = 0;
			Strings = 16;
			NodesPerString = 50;
			LightSize = 2;
			TopRadius = 10;
			BottomRadius = 100;					
		}

		#endregion

		#region Public Override Methods

		override public string GetSummary()
		{
			string Summary =
				"<style>" +
				$"  h2   {{color: #{new RGB(ThemeColorTable.ForeColor).ToHex()}; margin-top: 0; margin-bottom: 0; text-decoration: underline;}}" +
				$"  body {{color: #{new RGB(ThemeColorTable.ForeColor).ToHex()}; margin-top: 0;}}" +
				$"  b    {{color: #{new RGB(ThemeColorTable.ForeColorDisabled).ToHex()}; margin-left: 20px;}}" +
				"</style>" +
				$"<h2>Basic Attributes</h2>" +
				"<body>" +
				$"<b>Prop Type:</b> {PropType.Tree.GetEnumDescription()}<br>" +
				$"<b>Name:</b> {Name}<br>" +
				$"<b>Strings:</b> {Strings}<br>" +
				$"<b>Nodes per String:</b> {NodesPerString}<br>" +
				$"<b>Light Size:</b> {LightSize}<br>" +
				$"<b>Degrees Coverage:</b> {DegreesCoverage}<br>" +
				$"<b>Degree offset:</b> {DegreeOffset}<br>" +
				$"<b>Base Height:</b> {BaseHeight}<br>" +
				$"<b>Top Height:</b> {TopHeight}<br>" +
				$"<b>Top Width:</b> {TopWidth}<br>" +
				$"<b>Nodes per String:</b> {NodesPerString}<br>" +
				$"<b>Start Location:</b> {EnumValueTypeConverter.GetDescription(StartLocation)}<br>" +
				$"<b>ZigZag:</b> {ZigZag}<br>" +
				$"<b>ZigZag Offset:</b> {ZigZagOffset}<br>" +
				$"<b>Top Radius:</b> {TopRadius}<br>" +
				$"<b>Bottom Radius:</b> {BottomRadius}<br>" +
				$"<b>{AxisRotations[0].Axis} Rotation:</b> {AxisRotations[0].RotationAngle}\u00B0<br>" +
				$"<b>{AxisRotations[1].Axis} Rotation:</b> {AxisRotations[1].RotationAngle}\u00B0<br>" +
				$"<b>{AxisRotations[2].Axis} Rotation:</b> {AxisRotations[2].RotationAngle}\u00B0<br>" +
				"</body>" +
				"<h2>Additional Props</h2>" +
				"<body>" +
				//				$"<b>Left and Right Tree:</b> {LeftRight}<br>" +
				"</body>" +
				GetDimmingSummary() +
				GetColorSummary() +
				GetBaseSummary();

			return Summary;
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// The number of light strings
		/// </summary>
		public int Strings
		{
			get => PropModel.Strings;
			set
			{
				if (value <= 0) return;
				PropModel.Strings = value;
				OnPropertyChanged(nameof(Strings));
			}
		}

		/// <summary>
		/// The number of light nodes per string
		/// </summary>
		public int NodesPerString
		{
			get => PropModel.NodesPerString;
			set
			{
				if (value <= 0) return;
				if (value == PropModel.NodesPerString)
				{
					return;
				}
				PropModel.NodesPerString = value;
				OnPropertyChanged(nameof(NodesPerString));
			}
		}

		/// <summary>
		/// The degrees of coverage for the Tree. ex. 180 for a half tree.
		/// </summary>
		
		public int DegreesCoverage
		{
			get => PropModel.DegreesCoverage;
			set
			{
				if (value > 360 || value <= 0) return;
				if (value == PropModel.DegreesCoverage)
				{
					return;
				}
				PropModel.DegreesCoverage = value;
				OnPropertyChanged(nameof(DegreesCoverage));
			}
		}

		/// <summary>
		/// Offset in the rotation of where string one occurs in degrees.
		/// </summary>
		private int _degreesOffset;
		public int DegreeOffset
		{
			get => PropModel.DegreesOffset;
			set
			{
				if (value > 359 || value < -359) return;
				if (value == PropModel.DegreesOffset)
				{
					return;
				}

				PropModel.DegreesOffset = value;
				OnPropertyChanged(nameof(DegreeOffset));
			}
		}

	
		public int BaseHeight
		{
			get => PropModel.BaseHeight;
			set
			{
				if (value <= 0 || value == PropModel.BaseHeight)
				{
					return;
				}

				PropModel.BaseHeight = value;
				OnPropertyChanged(nameof(BaseHeight));
			}
		}

	
		public int TopHeight
		{
			get => PropModel.TopHeight;
			set
			{
				if (value <= 0 || value == PropModel.TopHeight)
				{
					return;
				}

				PropModel.TopHeight = value;
				OnPropertyChanged(nameof(TopHeight));
			}
		}

		private int _topWidth;
		public int TopWidth
		{
			get => PropModel.TopWidth;
			set
			{
				if (value <= 0 || value == PropModel.TopHeight)
				{
					return;
				}

				PropModel.TopHeight = value;
				OnPropertyChanged(nameof(TopWidth));
			}
		}

		private StartLocation _startLocation;
		public StartLocation StartLocation
		{
			get => _startLocation;
			set => SetProperty(ref _startLocation, value);
		}

		private bool _zigZag;
		public bool ZigZag
		{
			get => _zigZag;
			set => SetProperty(ref _zigZag, value);
		}

		private int _zigZagOffset;
		public int ZigZagOffset
		{
			get => _zigZagOffset;
			set
			{
				if (value <= 0) return;
				SetProperty(ref _zigZagOffset, value);
			}
		}

		/// <summary>
		/// Top radius of the tree as a percentage.
		/// </summary>
		public float TopRadius
		{
			get => PropModel.TopRadius;
			set
			{
				PropModel.TopRadius = value;
				OnPropertyChanged(nameof(TopRadius));
			}
		}

		/// <summary>
		/// Bottom radius of the tree as a percentage.
		/// </summary>
		public float BottomRadius
		{
			get => PropModel.BottomRadius;
			set
			{
				PropModel.BottomRadius = value;
				OnPropertyChanged(nameof(BottomRadius));
			}
		}
		
		//TODO Map element structure to model nodes
					
		#endregion

		#region Private Methods
		
		private void UpdateDefaultPropComponents()
		{

			CreateOrUpdateStringPropComponents();
			CreateOrUpdateTreeHalfPropComponents();
			//TODO Validate user defined components to see if they are still valid
		}

		private void CreateOrUpdateStringPropComponents()
		{
			var head = GetOrCreateElementNode();
			var nameIndex = AutoPropPrefix.Length;
			if (!PropComponents.Any())
			{
				//Add each string as a component
				foreach (var stringNode in head.Children)
				{
					var propComponent = PropComponentManager.CreatePropComponent($"{Name} {stringNode.Name.Substring(nameIndex)}", Id, PropComponentType.PropDefined);
					propComponent.TryAdd(stringNode);
					PropComponents.Add(propComponent);
				}
			}
			else
			{
				//Remove components that no longer exist
				var componentsToRemove = PropComponents.Where(x => !head.Children.Any(y => x.TargetNodes.Contains(y)))
					.ToList();
				foreach (var component in componentsToRemove)
				{
					PropComponents.Remove(component);
				}

				//Add new components if any
				foreach (var stringNode in head.Children)
				{
					if (!PropComponents.Any(x => x.TargetNodes.Contains(stringNode)))
					{
						var propComponent = PropComponentManager.CreatePropComponent($"{Name} {stringNode.Name.Substring(nameIndex)}", Id, PropComponentType.PropDefined);
						propComponent.TryAdd(stringNode);
						PropComponents.Add(propComponent);
					}
				}
			}
		}

		private void CreateOrUpdateTreeHalfPropComponents()
		{
			var head = GetOrCreateElementNode();

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

			int middle = (int)Math.Round(Strings / 2d, MidpointRounding.AwayFromZero);
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

			//Add them to a PropComponentNode so the user can do something useful
			//TODO add logic to clean this up if/when we are deleted.
			var parentPropComponentNode = VixenSystem.PropComponents.CreatePropComponentNode($"{Name} Halves");
			VixenSystem.PropComponents.AddPropComponent(propComponentLeft, parentPropComponentNode);
			VixenSystem.PropComponents.AddPropComponent(propComponentRight, parentPropComponentNode);
		}

		#endregion

		#region Protected Methods

		protected async void Prop_PropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			try
			{			
				// Name is handled in the base class so we need to handle our own.
				if (e.PropertyName != null)
				{
					// Call base class implementation
					base.Prop_PropertyChanged(sender, e);

					switch (e.PropertyName)
					{
						case nameof(StartLocation):
						case nameof(ZigZag):
						case nameof(ZigZagOffset):
							await AddOrUpdatePatchingOrder(_startLocation, _zigZag, _zigZagOffset);
							break;

						case nameof(NodesPerString):
						case nameof(TopRadius):
						case nameof(BottomRadius):
						case nameof(DegreesCoverage):
						case nameof(DegreeOffset):
						case nameof(Strings):
							GenerateDebouncer.Debounce();
							break;
					}
				}
			}
			catch (Exception ex)
			{
				Logging.Error(ex, $"An error occured handling Tree property {e.PropertyName} changed");
			}
		}
		
		protected override async Task GenerateElementsAsync()
		{
			bool hasUpdatedStrings = false;
			bool hasUpdatedNodes = false;

			try
			{
				var propNode = GetOrCreateElementNode();
				if (propNode.IsLeaf && Strings > 0)
				{
					AddStringElements(propNode, Strings, NodesPerString);
					hasUpdatedStrings = true;
				}
				else if (propNode.Children.Count() != Strings)
				{
					await UpdateStrings(Strings).ConfigureAwait(false);
					hasUpdatedStrings = true;
				}

				if (propNode.Children.Any())
				{
					if (propNode.Children.First().Children.Count() != NodesPerString)
					{
						await UpdateNodesPerString(NodesPerString).ConfigureAwait(false);
						hasUpdatedStrings = true;
					}
				}

				if (hasUpdatedStrings || hasUpdatedNodes)
				{
					await AddOrUpdatePatchingOrder(_startLocation, _zigZag, _zigZagOffset)
						.ConfigureAwait(false);

					await AddOrUpdateColorHandling().ConfigureAwait(false);
				}

				if (hasUpdatedStrings)
				{
					UpdateDefaultPropComponents();
				}

			}
			catch (Exception e)
			{
				Logging.Error(e, "An exception occured creating the prop");
			}

		}

		#endregion
	}
}
