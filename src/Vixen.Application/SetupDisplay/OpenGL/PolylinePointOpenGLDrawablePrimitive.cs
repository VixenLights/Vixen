using Common.OpenGLCommon.Constructs.DrawingEngine.Primitive;

namespace VixenApplication.SetupDisplay.OpenGL
{
	/// <summary>
	/// Maintains a polyline point.
	/// </summary>
	public class PolylinePointOpenGLDrawablePrimitive : OpenGLDrawablePrimitive
	{
		#region Public Properties

		/// <summary>
		/// X position of the point in world coordinates.
		/// </summary>
		public float WorldPtX { get; set; }

		/// <summary>
		/// Y position of the point in world coordinates.
		/// </summary>
		public float WorldPtY { get; set; }

		#endregion

		#region Public Methods

		/// <summary>
		/// Creates the point square verticees.
		/// </summary>
		/// <param name="x">X position of square</param>
		/// <param name="y">Y position of square</param>
		public void CreatePoint(float x, float y)
		{
			int SizeOfPt = 5;

			// Clear the existing vertices
			Vertices.Clear();
			
			// Top Left
			Vertices.Add(x - SizeOfPt);
			Vertices.Add(y + SizeOfPt);
			Vertices.Add(0.0f);

			// Top Right
			Vertices.Add(x + SizeOfPt);
			Vertices.Add(y + SizeOfPt);
			Vertices.Add(0.0f);

			// Bottom Right
			Vertices.Add(x + SizeOfPt);
			Vertices.Add(y - SizeOfPt);
			Vertices.Add(0.0f);

			// Bottom Left
			Vertices.Add(x - SizeOfPt);
			Vertices.Add(y - SizeOfPt);
			Vertices.Add(0.0f);
		}

		#endregion
	}
}
