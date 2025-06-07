#nullable enable

using System.ComponentModel;
using System.Windows.Markup;
using NLog;
using Vixen.Attributes;
using Vixen.Services;
using Vixen.Sys;
using Vixen.Sys.Props;
using Vixen.Utility;
using VixenModules.Property.Order;

namespace VixenModules.App.Props.Models.Tree
{
    public class Tree: BaseProp, IProp 
	{
        private static readonly Logger Logging = LogManager.GetCurrentClassLogger();

        private TreeModel _propModel;
        private StartLocation _startLocation;
        private bool _zigZag;
        private int _zigZagOffset;
        private Guid _rootElementNodeId = Guid.Empty;

        public Tree() : this("Tree 1", 16, 50)
        {

        }

        public Tree(string name, int strings, int nodesPerString) : this(name, strings, nodesPerString, StringTypes.Pixel)
        {

        }

        public Tree(string name, int strings = 16, int nodesPerString = 50, StringTypes stringType = StringTypes.Pixel) : base(name, PropType.Arch)
        {
            PropType = PropType.Tree;
            Name = name;
            StringType = stringType;
            StartLocation = StartLocation.BottomLeft;
			
			// Create Preview model
			TreeModel model = new TreeModel(strings, nodesPerString);
            _propModel = model;
			_propModel.PropertyChanged += _propModel_PropertyChanged;
			PropertyChanged += Tree_PropertyChanged; ;
			// Create default element structure
			Task.Run(GenerateElementsAsync).Wait();
		}

		private async void Tree_PropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
            if (nameof(Name).Equals(e.PropertyName))
            {
                RenamePropElement(AutoPropName);
            }

            if (nameof(StringType).Equals(e.PropertyName))
            {
                await GenerateElementsAsync();
            }
		}

		private async void _propModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            await GenerateElementsAsync();
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
                if(value <= 0) return;
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

        private void RenamePropElement(string name)
        {
            var element = VixenSystem.Nodes.GetElementNode(_rootElementNodeId);
            if (element != null)
            {
                VixenSystem.Nodes.RenameNode(element, name, false);
            }
        }

        private async Task<IEnumerable<ElementNode>> GenerateElementsAsync()
		{
            if (_rootElementNodeId != Guid.Empty)
            {
                //Remove our old tree for now. Eventually we should be smarter about reconstructing the node tree.
                var elementNodeToRemove = VixenSystem.Nodes.GetElementNode(_rootElementNodeId);
                if (elementNodeToRemove != null)
                {
                    VixenSystem.Nodes.RemoveNode(elementNodeToRemove,VixenSystem.Nodes.PropRootNode,true);
                }
            }
			List<ElementNode> result = new List<ElementNode>();

			if (Name.Length == 0)
			{
				Logging.Error("Tree Name is null");
				return await Task.FromResult(result);
			}

			if (Strings < 0)
			{
				Logging.Error("Negative string count");
				return await Task.FromResult(result);
			}

			if (StringType == StringTypes.Pixel && NodesPerString < 0)
			{
				Logging.Error("Negative nodes per string");
				return await Task.FromResult(result);
			}

			ElementNode head = ElementNodeService.Instance.CreateSingle(VixenSystem.Nodes.PropRootNode, AutoPropStringName, true, false);
            _rootElementNodeId = head.Id;
            result.Add(head);

			for (int i = 0; i < Strings; i++)
			{
				string stringName = $"{AutoPropStringName} {i+1}";
				ElementNode stringNode = ElementNodeService.Instance.CreateSingle(head, stringName, true, false);
				result.Add(stringNode);

				if (StringType == StringTypes.Pixel)
				{
					for (int j = 0; j < NodesPerString; j++)
					{
						string pixelName = $"{AutoPropNodeName} {j + 1}";

						ElementNode pixelNode = ElementNodeService.Instance.CreateSingle(stringNode, pixelName, true, false);
						result.Add(pixelNode);
					}
				}
			}

			IEnumerable<ElementNode> leafNodes = [];

			if (_startLocation == StartLocation.BottomLeft)
			{
                leafNodes = result.First().GetLeafEnumerator();
				if (_zigZag)
				{
					OrderModule.AddPatchingOrder(leafNodes, ZigZagOffset);
                    return result;
				}
			}

			if (_startLocation == StartLocation.BottomRight)
			{
				leafNodes = result.First().Children.SelectMany(x => x.GetLeafEnumerator().Reverse());
			}
			else if (_startLocation == StartLocation.TopLeft)
			{
				leafNodes = result.First().Children.Reverse().SelectMany(x => x.GetLeafEnumerator());
			}
			else if (_startLocation == StartLocation.TopRight)
			{
				leafNodes = result.First().GetLeafEnumerator().Reverse();
			}

			if (_zigZag)
			{
				OrderModule.AddPatchingOrder(leafNodes, ZigZagOffset);
			}
			else
			{
				OrderModule.AddPatchingOrder(leafNodes);
			}

			return await Task.FromResult(result);
		}

	}
}