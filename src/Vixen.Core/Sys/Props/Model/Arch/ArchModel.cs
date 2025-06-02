using System.Diagnostics;
using Vixen.Extensions;

namespace Vixen.Sys.Props.Model.Arch
{
	public class ArchModel: BaseLightModel
	{
        
        private int _nodeSize;
        private int _nodeCount;

        public ArchModel():this(25)
        {
            
        }

        public ArchModel(int nodeCount, int nodeSize = 2)
        {
            _nodeCount = nodeCount;
            _nodeSize = nodeSize;
            Nodes.AddRange(GetArchPoints(_nodeCount, RotationAngle));
			PropertyChanged += ArchModel_PropertyChanged;
			Nodes.CollectionChanged += Nodes_CollectionChanged;
        }

		private void Nodes_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			Debug.WriteLine("Nodes Changed");
		}

		private void ArchModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
            //TODO make this smarter to do the minimal to add, subtract, or update node size or rotation angle.
            Nodes.Clear();
            Nodes.AddRange(GetArchPoints(_nodeCount, RotationAngle));
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

        public static List<NodePoint> GetArchPoints(double numPoints, int rotationAngle)
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

            if (rotationAngle != 0)
            {
                RotateNodePoints(vertices, rotationAngle);
            }

            return vertices;
        }
	}
}
