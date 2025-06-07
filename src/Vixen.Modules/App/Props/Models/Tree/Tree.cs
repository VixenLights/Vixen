#nullable enable

using System.ComponentModel;
using NLog;
using Vixen.Attributes;
using Vixen.Services;
using Vixen.Sys;
using Vixen.Sys.Props;
using Vixen.Utility;

namespace VixenModules.App.Props.Models.Tree
{
    public class Tree: BaseProp, IProp 
	{
        private static readonly Logger Logging = LogManager.GetCurrentClassLogger();

        private TreeModel _propModel;
        private StartLocation _startLocation;
        private bool _zigZag;
        private int _zigZagOffset;

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
			//TODO create default element structure
			//TODO create Preview model
			TreeModel model = new TreeModel(strings, nodesPerString);
            _propModel = model;
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
                if (value == _propModel.BaseHeight)
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
                if (value == _propModel.TopHeight)
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
                if (value == _propModel.TopHeight)
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
            set => SetProperty(ref _zigZagOffset, value);
        }

        public async Task<IEnumerable<ElementNode>> GenerateElements(IEnumerable<ElementNode>? selectedNodes = null)
		{
			List<ElementNode> result = new List<ElementNode>();

			if (Name.Length == 0)
			{
				Logging.Error("treename is null");
				return await Task.FromResult(result);
			}

			if (Strings < 0)
			{
				Logging.Error("negative count");
				return await Task.FromResult(result);
			}

			if (StringType == StringTypes.Pixel && NodesPerString < 0)
			{
				Logging.Error("negative pixelsperstring");
				return await Task.FromResult(result);
			}

			//Optimize the name check for performance. We know we are going to create a bunch of them and we can handle it ourselves more efficiently
			HashSet<string> elementNames = new HashSet<string>(VixenSystem.Nodes.Select(x => x.Name));

			ElementNode head = ElementNodeService.Instance.CreateSingle(null, NamingUtilities.Uniquify(elementNames, Name), true, false);
			result.Add(head);

			for (int i = 0; i < Strings; i++)
			{
				string stringname = head.Name + " " + "S" + (i + 1);
				ElementNode stringNode = ElementNodeService.Instance.CreateSingle(head, NamingUtilities.Uniquify(elementNames, stringname), true, false);
				result.Add(stringNode);

				if (StringType == StringTypes.Pixel)
				{
					for (int j = 0; j < NodesPerString; j++)
					{
						string pixelname = stringNode.Name + " " + "Px" + (j + 1);

						ElementNode pixelnode = ElementNodeService.Instance.CreateSingle(stringNode, NamingUtilities.Uniquify(elementNames, pixelname), true, false);
						result.Add(pixelnode);
					}
				}
			}

			IEnumerable<ElementNode> leafNodes = Enumerable.Empty<ElementNode>();

			if (_startLocation == StartLocation.BottomLeft)
			{
				if (_zigZag)
				{
					leafNodes = result.First().GetLeafEnumerator();
					//OrderModule.AddPatchingOrder(leafNodes, _zigZagEvery);
				}

				return result;
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
				//OrderModule.AddPatchingOrder(leafNodes, _zigZagEvery);
			}
			else
			{
				//OrderModule.AddPatchingOrder(leafNodes);
			}

			return await Task.FromResult(result);
		}

	}
}