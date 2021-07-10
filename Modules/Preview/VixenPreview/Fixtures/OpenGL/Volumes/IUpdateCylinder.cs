namespace VixenModules.Preview.VixenPreview.Fixtures.OpenGL.Volumes
{
	/// <summary>
	/// Updates the geometry of a cylinder.
	/// </summary>
	public interface IUpdateCylinder
	{
		/// <summary>
		/// Updates the geometry of a cylinder.
		/// </summary>		
		/// <param name="yOffset">Y offset of the cylinder</param>
		/// <param name="length">Length of the cylinder</param>
		/// <param name="bottomRadius">Bottom radius of the cylinder</param>
		/// <param name="topRadius">Top radius of the cylinder</param>
		void Update(
			float yOffset,
			float length,
			float bottomRadius,
			float topRadius);			
	}
}
