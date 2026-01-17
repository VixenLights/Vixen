using OpenTK.Mathematics;
using VixenModules.Editor.FixtureGraphics.OpenGL.Volumes;

namespace VixenModules.Editor.FixtureGraphics.OpenGL
{
	/// <summary>
	/// Renders a 3-D moving head using OpenGL.
	/// </summary>
	public interface IRenderMovingHeadOpenGL
	{
		/// <summary>
		/// Initializes the volumes that make up the moving head.
		/// </summary>
		/// <param name="length">Width and height of the drawing area</param>
		/// <param name="maxBeamLength">Maximum length of the light beam</param>
		/// <param name="beamTransparency">Transparency of the light beam</param>
		/// <param name="beamWidthMultiplier">Beam width as a multiplier of the base beam width</param>
		/// <param name="mountingPosition">Mounting position of the fixture</param>
		void Initialize(double length, double maxBeamLength, double beamTransparency, int beamWidthMultiplier, MountingPositionType mountingPosition);

		/// <summary>
		/// Gets the graphical volumes associated with the moving head.
		/// </summary>
		/// <returns>Graphical volumes that make up the moving head</returns>
		IEnumerable<Tuple<IVolume, Guid>> GetVolumes();

		/// <summary>
		/// Gets the graphical volumes that make up the physical intelligent fixture.
		/// Does not include any beam volumes.
		/// </summary>
		/// <returns>Volumes that make up the physical fixture</returns>
		IEnumerable<IVolume> GetPhysicalFixtureVolumes();

		/// <summary>
		/// Draws the legend for the moving head.
		/// </summary>
		/// <param name="translateX">X Position of the moving head</param>
		/// <param name="translateY">Y Position of the moving head</param>
		/// <param name="projectionMatrix">Projection matrix used for rendering in 3-D</param>
		/// <param name="viewMatrix">View matrix for rendering in 3-D</param>
		void DrawLegend(
			int translateX,
			int translateY,
			Matrix4 projectionMatrix,
			Matrix4 viewMatrix);
	}
}
