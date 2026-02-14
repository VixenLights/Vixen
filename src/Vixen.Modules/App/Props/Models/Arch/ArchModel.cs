using Catel.Reflection;
using System.Collections.ObjectModel;
using Vixen.Extensions;
using Vixen.Sys.Props.Model;
using VixenApplication.SetupDisplay.Wizards.HelperTools;
using VixenModules.App.Props.Models;


namespace VixenModules.App.Props.Models.Arch
{
	/// <summary>
	/// Maintains an Arch prop model.
	/// </summary>
	public class ArchModel: BaseLightModel
	{
		public ArchModel()
		{
			PropParameters.Update("NodeCount", 3);
			PropParameters.Update("LightSize", 2);
			Nodes = new(Get3DNodePoints());
			PropertyChanged += PropertyModelChanged;
		}

		/// <summary>
		/// Calculates the 3-D points that make up the arch.
		/// </summary>
		/// <returns>Collection of node points that make up the arch</returns>
		protected override IEnumerable<NodePoint> Get3DNodePoints()
		{
			int numPoints = (int)PropParameters.Get("NodeCount");
			int size = (int)PropParameters.Get("LightSize");
			ObservableCollection<AxisRotationModel> rotations = (ObservableCollection<AxisRotationModel>)PropParameters.Get("Rotations");

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
			RotatePoints(vertices, rotations);

			return vertices;
		}
	}
}
