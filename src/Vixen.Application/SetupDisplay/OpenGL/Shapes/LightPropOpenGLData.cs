using OpenTK.Mathematics;

using Vixen.Sys.Props.Model;

using VixenApplication.SetupDisplay.OpenGL.Shapes;

namespace VixenApplication.SetupDisplay.OpenGL
{
	/// <summary>
	/// Maintains OpenGL data structures required to draw a light based prop.
	/// </summary>
	public class LightPropOpenGLData : PropOpenGLData, ILightPropOpenGLData
	{
		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="lightPropModel">Model associated with the prop</param>
		public LightPropOpenGLData(ILightPropModel lightPropModel) : base(lightPropModel)
		{
			// Store off the light prop model
			_propModel = lightPropModel;			
		}

		#endregion

		#region Fields

		/// <summary>
		/// Collection of point vertices to be drawn.
		/// </summary>
		private List<float> _points = new List<float>();		

		/// <summary>
		/// Model associated with the light based prop.
		/// </summary>
		private ILightPropModel _propModel;

		#endregion

		#region ILightPropOpenGLData

		/// <inheritdoc/>		
		public void UpdateDrawPoints(float controlHeight)
		{
			// Clear the points
			_points.Clear();

			// Create the color points
			CreateFullColorPoints(controlHeight);
		}
						
		#endregion

		#region Private Methods

		/// <summary>
		/// Creates the vertex data required to draw the light points.
		/// </summary>
		/// <param name="controlHeight">Logical height of the view</param>
		private void CreateFullColorPoints(float controlHeight)
		{												
			List<Vector3> coordinates = new List<Vector3>();
			
			// Loop over the 3-D points associated with the prop
			foreach (NodePoint nodePoint in _propModel.Nodes)
			{				
				// Adjust (scale) the 3-D points for the reference height
				_points.Add((float)(nodePoint.X * SizeX) + X);
				_points.Add((float)(nodePoint.Y * SizeY) + Y);						
				_points.Add((float)(nodePoint.Z * SizeZ));

				Vector3 vPoint = new Vector3();
				vPoint.X = (float)(nodePoint.X * SizeX + X);
				vPoint.Y = (float)(nodePoint.Y * SizeY + Y);
				vPoint.Z = (float)(nodePoint.Z * SizeZ);

				coordinates.Add(vPoint);

				// If the prop is selected then...
				if (Selected)
				{
					// Configure the vertex as Lime Green
					_points.Add(Color.LimeGreen.R);  // R
					_points.Add(Color.LimeGreen.G);  // G
					_points.Add(Color.LimeGreen.B);  // B
					_points.Add(0xff); // Alpha (Brightness)
				}
				else
				{
					// Configure the vertex as Turquoise
					_points.Add(Color.Turquoise.R); // R
					_points.Add(Color.Turquoise.G); // G
					_points.Add(Color.Turquoise.B); // B
					_points.Add(0xff); // Alpha (Brightness)
				}
					
				// Add the light point size
				_points.Add(nodePoint.Size);				
			}

			// Clear the previous frame points
			Vertices.Clear();
			Vertices.AddRange(_points);

			// Initialize the vertices used to indicate the prop is selected
			// TODO: Should this be optimized to only run when the prop is actually selected
			InitializeSelectionVertices(coordinates, SizeY);			
		}

		#endregion						
	}
}
