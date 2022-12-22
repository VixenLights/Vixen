namespace VixenModules.Editor.FixtureGraphics.OpenGL.Volumes
{
	/// <summary>
	/// Defines a cylinder with a configurable bottom and top radius.
	/// </summary>
	public class Cylinder : CylinderBase
	{
		#region Constructors

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
				Vertices,
				AllPanelsIndices);
		}

		/// <summary>
		/// Static Constructor
		/// </summary>
		static Cylinder()
		{
			// Default to drawing all 20 panels
			AllPanelsIndices = new List<int>()
				{ 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19 };
		}

		#endregion

		#region Putlic Static Properties

		/// <summary>
		/// Collection of all 20 panel indices that make up the cylinder.
		/// </summary>
		public static List<int> AllPanelsIndices { get; private set; }

		#endregion
	}
}
