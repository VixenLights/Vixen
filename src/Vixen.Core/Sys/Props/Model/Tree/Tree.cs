#nullable enable

using System.ComponentModel;
using Vixen.Attributes;

namespace Vixen.Sys.Props.Model.Tree
{
    public class Tree: BaseProp, IProp 
	{
        private TreeModel _propModel;
        
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

    }
}