#nullable enable

using System.ComponentModel;
using System.Drawing;
using NLog;
using Vixen.Attributes;
using Vixen.Sys;
using Vixen.Sys.Props;
using VixenModules.Property.Color;

namespace VixenModules.App.Props.Models.Tree
{
	public class Tree : BaseLightProp, IProp
	{
		private static readonly Logger Logging = LogManager.GetCurrentClassLogger();

		private TreeModel _propModel;
		private StartLocation _startLocation;
		private bool _zigZag;
		private int _zigZagOffset;

		public Tree() : this("Tree 1", 0, 0)
		{
			//Set initial to 0 so creation does not trigger element generation.
		}

		public Tree(string name, int strings, int nodesPerString) : this(name, strings, nodesPerString, StringTypes.Pixel)
		{

		}

		public Tree(string name, int strings = 16, int nodesPerString = 50, StringTypes stringType = StringTypes.Pixel) : base(name, PropType.Arch)
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
			// Create default element structure
			_ = GenerateElementsAsync();

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
                            await GenerateElementsAsync();
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
                            await UpdateStrings(Strings);
                            break;
                        case nameof(NodesPerString):
                            await UpdateNodesPerString(NodesPerString);
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
		[PropertyOrder(0)]
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
		[PropertyOrder(1)]
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
		[PropertyOrder(2)]
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
		[PropertyOrder(3)]
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
		[PropertyOrder(4)]
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
		[PropertyOrder(5)]
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
		[PropertyOrder(6)]
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
		[PropertyOrder(7)]
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
		[DisplayName("Start Location")]
		[PropertyOrder(8)]
		public StartLocation StartLocation
		{
			get => _startLocation;
			set => SetProperty(ref _startLocation, value);
		}

		[Category("Patching")]
		[DisplayName("Zig Zag")]
		[PropertyOrder(9)]
		public bool ZigZag
		{
			get => _zigZag;
			set => SetProperty(ref _zigZag, value);
		}

		[Category("Patching")]
		[DisplayName("Zig Zag Every")]
		[PropertyOrder(10)]
		public int ZigZagOffset
		{
			get => _zigZagOffset;
			set
			{
				if (value <= 0) return;
				SetProperty(ref _zigZagOffset, value);
			}
		}

		protected async Task<bool> GenerateElementsAsync()
		{
			while (UpdateInProgress)
			{
				await Task.Delay(500);
			}

			UpdateInProgress = true;

			ElementNode head = GetOrCreatePropElementNode();

			AddStringElements(head, Strings, NodesPerString);

			PropertySetupHelper.AddOrUpdatePatchingOrder(head, _startLocation, _zigZag, _zigZagOffset);

			await AddOrUpdateColorHandling();

			UpdateInProgress = false;

			return true;

		}

		private async Task AddOrUpdateColorHandling(IElementNode? node = null)
		{
			// Can this be raised up to the BaseProp at some point?
            try
            {
                var propNode = node ?? GetOrCreatePropElementNode();
                await PropertySetupHelper.AddOrUpdateColorHandling(propNode, GetColorConfiguration());
            }
            catch (Exception e)
            {
                Logging.Error(e, "Error occured updating the color handling");
            }
		}

        private async Task AddOrUpdatePatchingOrder()
        {
	        while (UpdateInProgress)
	        {
		        await Task.Delay(500);
	        }
	        UpdateInProgress = true;
            var propNode = GetOrCreatePropElementNode();
            PropertySetupHelper.AddOrUpdatePatchingOrder(propNode, _startLocation, _zigZag, _zigZagOffset);
            UpdateInProgress = false;
        }

	}
}