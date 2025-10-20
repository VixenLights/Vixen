using System.Collections.ObjectModel;
using Vixen.Extensions;
using Vixen.Sys.Props.Model;
using VixenModules.App.Props.Models;


namespace VixenModules.App.Props.Models.Arch
{
	/// <summary>
	/// Maintains an Arch prop model.
	/// </summary>
	public class ArchModel: BaseLightModel
	{
		IAttributeData _arch;

		public ArchModel()
		{
		}

		#region Abstract Overrides
		/// <inheritdoc/>				
		public override void SetContext(object data)
		{
			if (data is IAttributeData attributeData)
			{
				_arch = attributeData;
				Nodes.AddRange<NodePoint>(Get2DNodePoints());
				ThreeDNodes = new(Get3DNodePoints());
			}
			else
			{
				throw new ArgumentException("Invalid data type. Expected IAttributeData.", nameof(data));
			}
		}

		/// <inheritdoc/>				
		protected override IEnumerable<NodePoint> Get2DNodePoints()
		{
			int rotationAngle = _arch.Rotations != null ? _arch.Rotations[(int)Axis.ZAxis].RotationAngle : 0;
			return GetArchPoints(_arch.NodeCount, _arch.LightSize, rotationAngle);
		}

		/// <inheritdoc/>				
		protected override IEnumerable<NodePoint> Get3DNodePoints()
		{
			return Get3DNodePoints(_arch.NodeCount, _arch.LightSize);
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
				double x = (xScale + xScale * Math.Cos(t));
				double y = (yScale + yScale * Math.Sin(t));
				vertices.Add(new NodePoint(x, y) { Size = size });
				t += radianIncrement;
			}

			if (rotationAngle != 0)
            {
                RotateNodePoints(vertices, rotationAngle);
            }

            return vertices;
        }
		#endregion

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
			RotatePoints(vertices, AxisRotationViewModel.ConvertToModel(_arch.Rotations));

			return vertices;
		}
	}
}
