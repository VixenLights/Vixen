#nullable enable

using System.ComponentModel;
using AsyncAwaitBestPractices;
using Common.Controls.ColorManagement.ColorModels;
using Common.Controls.Theme;
using Common.WPFCommon.Converters;
using Debounce.Core;
using Vixen.Extensions;
using Vixen.Sys;
using Vixen.Sys.Managers;
using Vixen.Sys.Props;
using Vixen.Sys.Props.Components;

namespace VixenModules.App.Props.Models.Tree
{
	public class Tree : BaseLightProp<TreeModel>, IProp
	{
		#region Fields

		private readonly Debouncer _generateDebouncer;

		#endregion


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

			// Create Preview model
			PropModel = new TreeModel(strings, nodesPerString);

			_generateDebouncer = new Debouncer(() =>
			{
				GenerateElementsAsync().SafeFireAndForget();
			}, 500);
		}

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
				$"<b>{Rotations[0].Axis} Rotation:</b> {Rotations[0].RotationAngle}\u00B0<br>" +
				$"<b>{Rotations[1].Axis} Rotation:</b> {Rotations[1].RotationAngle}\u00B0<br>" +
				$"<b>{Rotations[2].Axis} Rotation:</b> {Rotations[2].RotationAngle}\u00B0<br>" +
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

		private async void Tree_PropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			try
			{
				// Name is handled in the base class so we need to handle our own.
				if (e.PropertyName != null)
				{
					switch (e.PropertyName)
					{
						case nameof(StringType):
							_generateDebouncer.Debounce();
							break;
						case nameof(StartLocation):
						case nameof(ZigZag):
						case nameof(ZigZagOffset):
							await AddOrUpdatePatchingOrder(_startLocation, _zigZag, _zigZagOffset);
							break;
					}
				}
			}
			catch (Exception ex)
			{
				Logging.Error(ex, $"An error occured handling Tree property {e.PropertyName} changed");
			}
		}

		private async void PropModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			try
			{
				if (e.PropertyName != null)
				{
					switch (e.PropertyName)
					{
						case nameof(Strings):
						case nameof(NodesPerString):
							_generateDebouncer.Debounce();
							break;
						case nameof(StartLocation):
						case nameof(ZigZag):
						case nameof(ZigZagOffset):
							await AddOrUpdatePatchingOrder();
							break;
					}
				}
			}
			catch (Exception ex)
			{
				Logging.Error(ex, $"An error occured handling model property {e.PropertyName} changed");
			}
		}

		/// <summary>
		/// The number of light strings
		/// </summary>
		private int _strings;
		public int Strings
		{
			get => _strings;
			set
			{
				if (value <= 0) return;
				SetProperty(ref _strings, value);
				OnPropertyChanged(nameof(Strings));
			}
		}

		/// <summary>
		/// The number of light nodes per string
		/// </summary>
		private int _nodesPerString;
		public int NodesPerString
		{
			get => _nodesPerString;
			set
			{
				if (value <= 0) return;
				if (value == _nodesPerString)
				{
					return;
				}
				SetProperty(ref _nodesPerString, value);
				OnPropertyChanged(nameof(NodesPerString));
			}
		}

		/// <summary>
		/// The degrees of coverage for the Tree. ex. 180 for a half tree.
		/// </summary>
		private int _degreesCoverage;
		public int DegreesCoverage
		{
			get => _degreesCoverage;
			set
			{
				if (value > 360 || value <= 0) return;
				if (value == _degreesCoverage)
				{
					return;
				}
				SetProperty(ref _degreesCoverage, value);
				OnPropertyChanged(nameof(DegreesCoverage));
			}
		}

		/// <summary>
		/// Offset in the rotation of where string one occurs in degrees.
		/// </summary>
		private int _degreesOffset;
		public int DegreeOffset
		{
			get => _degreesOffset;
			set
			{
				if (value > 359 || value < -359) return;
				if (value == _degreesOffset)
				{
					return;
				}

				SetProperty(ref _degreesOffset, value);
				OnPropertyChanged(nameof(DegreeOffset));
			}
		}

		private int _baseHeight;
		public int BaseHeight
		{
			get => _baseHeight;
			set
			{
				if (value <= 0)
				{
					return;
				}

				SetProperty(ref _baseHeight, value);
				OnPropertyChanged(nameof(BaseHeight));
			}
		}

		private int _topHeight;
		public int TopHeight
		{
			get => _topHeight;
			set
			{
				if (value <= 0)
				{
					return;
				}

				SetProperty(ref _topHeight, value);
				OnPropertyChanged(nameof(TopHeight));
			}
		}

		private int _topWidth;
		public int TopWidth
		{
			get => _topWidth;
			set
			{
				if (value <= 0)
				{
					return;
				}

				SetProperty(ref _topWidth, value);
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
		private float _topRadius;
		public float TopRadius
		{
			get => _topRadius;
			set
			{
				SetProperty(ref _topRadius, value);
				OnPropertyChanged(nameof(TopRadius));
			}
		}

		/// <summary>
		/// Bottom radius of the tree as a percentage.
		/// </summary>
		private float _bottomRadius;
		public float BottomRadius
		{
			get => _bottomRadius;
			set
			{
				SetProperty(ref _bottomRadius, value);
				OnPropertyChanged(nameof(BottomRadius));
			}
		}

		protected async Task GenerateElementsAsync()
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
	}
}