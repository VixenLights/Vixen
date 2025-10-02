using Vixen.Extensions;
using Vixen.Sys.Props.Model;

namespace VixenModules.App.Props.Models.Arch
{
	/// <summary>
	/// Maintains an Arch prop model.
	/// </summary>
	public class ArchModel: BaseLightModel
	{
		private Arch _arch;

		public ArchModel(Arch arch)
        {
			_arch = arch;
            Nodes.AddRange<NodePoint>(GetArchPoints(_arch.NodeCount, _arch.LightSize, _arch.Rotation));
        }

        public ArchModel(int nodeCount, int nodeSize = 2)
        {
            _nodeCount = nodeCount;
            _nodeSize = nodeSize;           
			Nodes = new(Get3DNodePoints());			
			PropertyChanged += PropertyModelChanged;

		#endregion

		#region Protected Abstract Overrides
				
		/// <inheritdoc/>				
		protected override IEnumerable<NodePoint> Get3DNodePoints()
		{
            //TODO make this smarter to do the minimal to add, subtract, or update node size or rotation angle.
            Nodes.Clear();
            Nodes.AddRange<NodePoint>(GetArchPoints(_nodeCount, _nodeSize, RotationAngle));
		}

		#endregion

		#region Private Methods
				
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
