using Vixen.Sys.Props.Model;

namespace VixenModules.App.Props.Models.Arch
{
	/// <summary>
	/// Maintains an Arch prop model.
	/// </summary>
	public class ArchModel: BaseLightModel
	{
		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		public ArchModel()
		{			
		}

		#endregion

		#region Public Properties

		private int _numPoints = 3;
		public int NumPoints
		{
			get => _numPoints;
			set => _numPoints = value;
		}

		#endregion

		#region Protected Overrides

		/// <summary>
		/// Calculates the 3-D points that make up the arch.
		/// </summary>
		/// <returns>Collection of node points that make up the arch</returns>
		protected override IEnumerable<NodePoint> Get3DNodePoints()
		{
			List<NodePoint> vertices = new List<NodePoint>();
			double xScale = .5f;
			double yScale = 1;
			double radianIncrement = Math.PI / (NumPoints - 1);

			double angle = Math.PI;
			
			// Loop until all points are defined
			while (vertices.Count < NumPoints)
			{
				// Calculate the position of the point
				double x = xScale + xScale * Math.Cos(angle);
				double y = yScale + yScale * Math.Sin(angle);

				// Adjust the point to a -0.5 to + 0.5 scale
				vertices.Add(new NodePoint(x - 0.5f, (1.0 - y) - 0.5f) { Size = LightSize });

				// Increase the angle of the next point
				angle += radianIncrement;
			}

			// (Optionally) rotate the points along the X, Y, and Z axis
			RotatePoints(vertices, AxisRotations);

			return vertices;
		}

		#endregion
	}
}
