namespace VixenApplication.SetupDisplay.OpenGL.Shapes
{
	/// <summary>
	/// Maintains state to find a poly point.
	/// </summary>
	public class PolyPointLocator
	{
		/// <summary>
		/// Index of the point on the poly-line.
		/// </summary>
		public int PointIndex { get; set; }

		/// <summary>
		/// The parent polyline prop of the point.
		/// </summary>
		public PolylinePropOpenGLData Prop { get; set; }

		/// <summary>
		/// A reference to the poly point.
		/// </summary>
		public PolylinePointOpenGLDrawablePrimitive Point { get; set; }
	}
}
