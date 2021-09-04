namespace VixenModules.Preview.VixenPreview.Fixtures.OpenGL.Volumes
{
	/// <summary>
	/// Defines the beam volume (rotating cylinder with end caps).
	/// </summary>
	public class BeamRotatingCylinderWithEndCaps : RotatingCylinderWithEndCaps, ISpecifyVolumeTransparency
	{
		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="yOffset">Y offset of the volume</param>
		/// <param name="length">Length of the cylinder</param>
		/// <param name="bottomRadius">Radius of the bottom of the cylinder</param>
		/// <param name="topRadius">Radius at the top of the cylinder</param>
		/// <param name="isDynamic">Whether the cylinder changes shape during execution</param>
		public BeamRotatingCylinderWithEndCaps(float yOffset, float length, float bottomRadius, float topRadius, bool isDynamic) :
			base(yOffset, length, bottomRadius, topRadius, isDynamic)
		{
			// Default to opaque
			Transparency = 0.0;
		}

		#endregion

		#region ISpecifyVolumeTransparency

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>
		public double Transparency 
		{ 
			get; 
			set; 
		}

		#endregion
	}
}
