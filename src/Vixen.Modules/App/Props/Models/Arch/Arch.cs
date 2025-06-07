
#nullable enable
using System.ComponentModel;
using Vixen.Attributes;
using Vixen.Sys.Props;

namespace VixenModules.App.Props.Models.Arch
{
    /// <summary>
    /// A class that defines an Arch Prop
    /// </summary>
    public class Arch: BaseProp, IProp
    {
        private ArchModel _propModel;
        
        public Arch():this("Arch 1", 25)
        {
            
        }

        public Arch(string name, int nodeCount): this(name, nodeCount, StringTypes.Pixel)
        {
            
        }

        public Arch(string name, int nodeCount = 25, StringTypes stringType = StringTypes.Pixel) : base(name, PropType.Arch)
        {
            //TODO create default element structure
            StringType = stringType;
            ArchModel model = new ArchModel(nodeCount);
            _propModel = model;
        }

        [Browsable(false)]
		IPropModel IProp.PropModel => PropModel;

		[Browsable(false)]
        public new ArchModel PropModel
        {
            get => _propModel;
            protected set => SetProperty(ref _propModel, value);
        }

        [DisplayName("Nodes Count")]
        [PropertyOrder(0)]
		public int NodeCount
        {
            get => _propModel.NodeCount;
            set
            {
                if (value == _propModel.NodeCount)
                {
                    return;
                }

                _propModel.NodeCount = value;
                OnPropertyChanged(nameof(NodeCount));
            }
        }

        [DisplayName("Nodes Size")]
        [PropertyOrder(1)]
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
	}
}