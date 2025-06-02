#nullable enable

namespace Vixen.Sys.Props.Model.Tree
{
    public class Tree: Prop
	{
        private int _strings;
        private int _nodesPerString;

        public Tree() : this("Tree 1", 16, 50)
        {

        }

        public Tree(string name, int strings, int nodesPerString) : this(name, strings, nodesPerString, StringTypes.Pixel)
        {

        }

        public Tree(string name, int strings = 16, int nodesPerString = 50, StringTypes stringType = StringTypes.Pixel) : base(name, PropType.Arch)
        {
            Name = name;
            _strings = strings;
            _nodesPerString = nodesPerString;
            StringType = stringType;
            //TODO create default element structure
            //TODO create Preview model
            TreeModel model = new TreeModel(Strings, nodesPerString);
            PropModel = model;
        }

        /// <summary>
        /// The number of light strings
        /// </summary>
        public int Strings
        {
            get => _strings;
            set => SetProperty(ref _strings, value);
        }

        /// <summary>
        /// The number of light nodes per string
        /// </summary>
        public int NodesPerString
        {
            get => _nodesPerString;
            set => SetProperty(ref _nodesPerString, value);
        }

        public int CoverageDegrees { get; set; }

        public int DegreeOffset { get; set; }

        public int BaseHeight { get; set; }

        public int TopHeight { get; set; }

        public int TopWidth { get; set; }
    }
}