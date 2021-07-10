namespace VixenModules.Preview.VixenPreview.Fixtures.OpenGL.Volumes
{
	/// <summary>
	/// Defines a cylinder with a configurable bottom and top radius.
	/// </summary>
	public class Cylinder : CylinderBase
	{
		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="length">Length of the cylinder</param>
		/// <param name="bottomRadius">Radius of the bottom of the cylinder</param>
		/// <param name="topRadius">Radius at the top of the cylinder</param>
		/// <param name="isDynamic">Whether the cylinder changes shape during execution</param>
		public Cylinder(float length, float bottomRadius, float topRadius, bool isDynamic) : base(isDynamic)
		{
			// Create the cylinder with the following parameters
			CreateCylinder(
				length,
				GetHorizontalCylinderPanelVertices,
				bottomRadius,
				topRadius,
				0.0f,
				false,
				Normals,
				Vertices);
		}

		#endregion
	}
}
