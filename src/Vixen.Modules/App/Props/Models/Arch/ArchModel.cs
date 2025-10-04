using Vixen.Extensions;
using Vixen.Sys.Props.Model;

namespace VixenModules.App.Props.Models.Arch
{
	/// <summary>
	/// Maintains an Arch prop model.
	/// </summary>
	public class ArchModel: BaseLightModel
	{                        
		#region Constructors

		/// <summary>
		/// Constructor
		/// </summary>
		public ArchModel():this(25)
        {
            
        }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="nodeCount">Number of nodes in the arch</param>
		/// <param name="nodeSize">Size of the nodes</param>
        public ArchModel(int nodeCount, int nodeSize = 2)
        {
            _nodeCount = nodeCount;
            _nodeSize = nodeSize;
            Nodes.AddRange<NodePoint>(Get2DNodePoints());
			ThreeDNodes = new(Get3DNodePoints());			
			PropertyChanged += PropertyModelChanged;
        }

		#endregion

		#region Protected Abstract Overrides
		
		/// <inheritdoc/>				
		protected override IEnumerable<NodePoint> Get2DNodePoints()
		{
			return GetArchPoints(_nodeCount, _nodeSize, RotationAngle);
		}

		/// <inheritdoc/>				
		protected override IEnumerable<NodePoint> Get3DNodePoints()
		{
			return Get3DNodePoints(_nodeCount, _nodeSize);
		}

		#endregion

		#region Public Properties

		private int _nodeSize;

		public int NodeSize
        {
            get => _nodeSize;
            set => SetProperty(ref _nodeSize, value);
        }

		private int _nodeCount;

		public int NodeCount
        {
            get => _nodeCount;
            set => SetProperty(ref _nodeCount, value);
        }

		#endregion

		#region Private Methods

		private static List<NodePoint> GetArchPoints(double numPoints, int size, int rotationAngle)
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
                vertices.Add(new NodePoint(x, y){Size = size});
                t += radianIncrement;
            }

            if (rotationAngle != 0)
            {
                RotateNodePoints(vertices, rotationAngle);
            }

            return vertices;
        }
		
		/// <summary>
		/// Calculates the 3-D points that make up the arch.
		/// </summary>
		/// <param name="numPoints">Number of node points in the arch</param>
		/// <param name="size">Size of the light</param>
		/// <returns>Collection of node points that make up the arch</returns>
		public List<NodePoint> Get3DNodePoints(double numPoints, int size)
		{
			List<NodePoint> vertices = new List<NodePoint>();
			double xScale = .5f;
			double yScale = 1;
			double radianIncrement = Math.PI / (numPoints - 1);

			double angle = Math.PI;
			
			// Loop until all points are defined
			while (vertices.Count < numPoints)
			{
				// Calculate the position of the point
				double x = xScale + xScale * Math.Cos(angle);
				double y = yScale + yScale * Math.Sin(angle);

				// Adjust the point to a -0.5 to + 0.5 scale
				vertices.Add(new NodePoint(x - 0.5f, (1.0 - y) - 0.5f) { Size = size });

				// Increase the angle of the next point
				angle += radianIncrement;
			}

			// (Optionally) rotate the points along the X, Y, and Z axis
			RotatePoints(vertices);

			return vertices;
		}

		#endregion
	}
}
