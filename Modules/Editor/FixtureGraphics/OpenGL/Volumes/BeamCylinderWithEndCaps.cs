using System.Collections.Generic;

namespace VixenModules.Editor.FixtureGraphics.OpenGL.Volumes
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
		/// <param name="panelIndices">Panel indices to draw</param>		
		public BeamRotatingCylinderWithEndCaps(float yOffset, float length, float bottomRadius, float topRadius, bool isDynamic, List<int> panelIndices) :
			base(yOffset, length, bottomRadius, topRadius, isDynamic, panelIndices)
		{
			// Default to opaque
			Transparency = 0.0;
		}

		/// <summary>
		/// Static constructor.
		/// </summary>
		static BeamRotatingCylinderWithEndCaps()
		{
			// Initialize the panel indices to draw the left half of the beam
			LeftHalfPanelIndices = new List<int>() { 5, 6, 7, 8, 9, 10, 11, 12, 13, 14 };

			// Initialize the panel indices to draw the right half of the beam
			RightHalfPanelIndices = new List<int>() { 0, 1, 2, 3, 4, 15, 16, 17, 18, 19 };
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

		#region Public Static Properties

		/// <summary>
		/// Collection of the panel indices that make up the left half of the beam.
		/// </summary>
		public static List<int> LeftHalfPanelIndices { get; private set; }

		/// <summary>
		/// Collection of the panel indices that make up the right half of the beam.
		/// </summary>
		public static List<int> RightHalfPanelIndices { get; private set; }

		#endregion
	}
}
