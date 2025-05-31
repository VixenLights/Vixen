using System.Collections.ObjectModel;
using Vixen.Model;

namespace Vixen.Sys.Props.Model.Arch
{
	public class ArchModel: BindableBase, ILightPropModel
	{
        private ObservableCollection<NodePoint> _nodes;
        private int _nodeSize;
        private int _nodeCount;

        public ArchModel():this(25)
        {
            
        }

        public ArchModel(int nodeCount, int nodeSize = 2)
        {
            _nodeCount = nodeCount;
            _nodeSize = nodeSize;
            _nodes = new ObservableCollection<NodePoint>(GetArcPoints(_nodeCount));
        }

		public Guid Id { get; init; } = Guid.NewGuid();

        public ObservableCollection<NodePoint> Nodes
        {
            get => _nodes;
            set => SetProperty(ref _nodes, value);
        }

        public int NodeSize
        {
            get => _nodeSize;
            set => SetProperty(ref _nodeSize, value);
        }

        public int NodeCount
        {
            get => _nodeCount;
            set => SetProperty(ref _nodeCount, value);
        }

        public static List<NodePoint> GetArcPoints(double numPoints)
        {
            List<NodePoint> vertices = new List<NodePoint>();
            double xScale = .5f;
            double yScale = 1;
            double radianIncrement = Math.PI / (numPoints - 1);

            double t = Math.PI;
            while (vertices.Count < numPoints)
            {
                double x = xScale + xScale * Math.Cos(t);
                double y = yScale + yScale * Math.Sin(t);
                vertices.Add(new NodePoint(x, y));
                t += radianIncrement;
            }
            return vertices;
        }
	}
}
