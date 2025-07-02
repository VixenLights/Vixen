#nullable enable

using NLog;
using System.ComponentModel;
using System.Diagnostics;
using AsyncAwaitBestPractices;
using Debounce.Core;
using Vixen.Attributes;
using Vixen.Sys;
using Vixen.Sys.Managers;
using Vixen.Sys.Props;
using Vixen.Sys.Props.Components;

namespace VixenModules.App.Props.Models.Tree
{
	public class Tree : BaseLightProp, IProp
	{
		private static readonly Logger Logging = LogManager.GetCurrentClassLogger();

		private TreeModel _propModel;
		private StartLocation _startLocation;
		private bool _zigZag;
		private int _zigZagOffset;
		private readonly Debouncer _generateDebouncer;
		

		public Tree() : this("Tree 1", 0, 0)
		{
			//Set initial to 0 so creation does not trigger element generation.
		}

		public Tree(string name, int strings, int nodesPerString) : this(name, strings, nodesPerString, StringTypes.Pixel)
		{

		}

		public Tree(string name, int strings = 0, int nodesPerString = 0, StringTypes stringType = StringTypes.Pixel) : base(name, PropType.Tree)
		{
			PropType = PropType.Tree;
			Name = name;
			StringType = stringType;
			ZigZagOffset = 50;
			StartLocation = StartLocation.BottomLeft;

			// Create Preview model
			TreeModel model = new TreeModel(strings, nodesPerString);
			_propModel = model;
			_propModel.PropertyChanged += PropModel_PropertyChanged;
			PropertyChanged += Tree_PropertyChanged;

			_generateDebouncer = new Debouncer(() =>
			{
				GenerateElementsAsync().SafeFireAndForget();
			}, 500);

			//TODO Map element structure to model nodes
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

		[Browsable(false)]
		IPropModel IProp.PropModel => PropModel;

		[Browsable(false)]
		public new TreeModel PropModel
		{
			get => _propModel;
			protected set => SetProperty(ref _propModel, value);
		}

		/// <summary>
		/// The number of light strings
		/// </summary>
		[DisplayName("String Count")]
		[PropertyOrder(10)]
		public int Strings
		{
			get => _propModel.Strings;
			set
			{
				if (value <= 0) return;
				_propModel.Strings = value;
				OnPropertyChanged(nameof(Strings));
			}
		}

		/// <summary>
		/// The number of light nodes per string
		/// </summary>
		[PropertyOrder(11)]
		[DisplayName("Nodes Per String")]
		public int NodesPerString
		{
			get => _propModel.NodesPerString;
			set
			{
				if (value <= 0) return;
				if (value == _propModel.NodesPerString)
				{
					return;
				}
				_propModel.NodesPerString = value;
				OnPropertyChanged(nameof(NodesPerString));
			}
		}

		[DisplayName("Nodes Size")]
		[PropertyOrder(12)]
		public int NodeSize
		{
			get => _propModel.NodeSize;
			set
			{
				if (value <= 0) return;
				if (value == _propModel.NodeSize)
				{
					return;
				}

				_propModel.NodeSize = value;
				OnPropertyChanged(nameof(NodeSize));
			}
		}

		/// <summary>
		/// The degrees of coverage for the Tree. ex. 180 for a half tree.
		/// </summary>
		[PropertyOrder(13)]
		[DisplayName("Degrees Coverage")]
		public int DegreesCoverage
		{
			get => _propModel.DegreesCoverage;
			set
			{
				if (value > 360 || value <= 0) return;
				if (value == _propModel.DegreesCoverage)
				{
					return;
				}
				_propModel.DegreesCoverage = value;
				OnPropertyChanged(nameof(DegreesCoverage));
			}
		}

		/// <summary>
		/// Offset in the rotation of where string one occurs in degrees.
		/// </summary>
		[DisplayName("Degree Offset")]
		[PropertyOrder(14)]
		public int DegreeOffset
		{
			get => _propModel.DegreesOffset;
			set
			{
				if (value > 359 || value < -359) return;
				if (value == _propModel.DegreesOffset)
				{
					return;
				}

				_propModel.DegreesOffset = value;
				OnPropertyChanged(nameof(DegreeOffset));
			}
		}

		[DisplayName("Base Height")]
		[PropertyOrder(15)]
		public int BaseHeight
		{
			get => _propModel.BaseHeight;
			set
			{
				if (value <= 0 || value == _propModel.BaseHeight)
				{
					return;
				}

				_propModel.BaseHeight = value;
				OnPropertyChanged(nameof(BaseHeight));
			}
		}

		[DisplayName("Top Height")]
		[PropertyOrder(16)]
		public int TopHeight
		{
			get => _propModel.TopHeight;
			set
			{
				if (value <= 0 || value == _propModel.TopHeight)
				{
					return;
				}

				_propModel.TopHeight = value;
				OnPropertyChanged(nameof(TopHeight));
			}
		}

		[DisplayName("Top Width")]
		[PropertyOrder(17)]
		public int TopWidth
		{
			get => _propModel.TopWidth;
			set
			{
				if (value <= 0 || value == _propModel.TopHeight)
				{
					return;
				}

				_propModel.TopHeight = value;
				OnPropertyChanged(nameof(TopWidth));
			}
		}

		[Category("Patching")]
		[DisplayName("Wiring Start")]
		[PropertyOrder(18)]
		public StartLocation StartLocation
		{
			get => _startLocation;
			set => SetProperty(ref _startLocation, value);
		}

		[Category("Patching")]
		[DisplayName("Zig Zag")]
		[PropertyOrder(19)]
		public bool ZigZag
		{
			get => _zigZag;
			set => SetProperty(ref _zigZag, value);
		}

		[Category("Patching")]
		[DisplayName("Zig Zag Every")]
		[PropertyOrder(20)]
		public int ZigZagOffset
		{
			get => _zigZagOffset;
			set
			{
				if (value <= 0) return;
				SetProperty(ref _zigZagOffset, value);
			}
		}

		protected async Task GenerateElementsAsync()
		{
			await Task.Factory.StartNew(async () =>
			{
				bool hasUpdatedStrings = false;
				bool hasUpdatedNodes = false;

				var propNode = GetOrCreatePropElementNode();
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
					await AddOrUpdatePatchingOrder(_startLocation, _zigZag, _zigZagOffset).ConfigureAwait(false);

					await AddOrUpdateColorHandling().ConfigureAwait(false);

					if (hasUpdatedStrings)
					{
						UpdateDefaultPropComponents();
					}
				}
			});
		}

		private void UpdateDefaultPropComponents()
		{
			
			CreateOrUpdateStringPropComponents();
			CreateOrUpdateTreeHalfPropComponents();
			//TODO Validate user defined components to see if they are still valid
		}

		private void CreateOrUpdateStringPropComponents()
		{
			var head = GetOrCreatePropElementNode();
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
			var head = GetOrCreatePropElementNode();

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
				propComponentRight = PropComponentManager.CreatePropComponent($"{Name} Right", Id,PropComponentType.PropDefined);
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