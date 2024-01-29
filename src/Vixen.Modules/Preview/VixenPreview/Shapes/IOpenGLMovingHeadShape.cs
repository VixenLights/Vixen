using VixenModules.Editor.FixtureGraphics.OpenGL;

namespace VixenModules.Preview.VixenPreview.Shapes
{
	/// <summary>
	/// Maintains a moving head preview shape that is composed of OpenGL graphical volumes.
	/// </summary>
	public interface IOpenGLMovingHeadShape
	{
		/// <summary>
		/// Initialize the moving head with the specified drawing area height.
		/// </summary>
		/// <remarks>The reference height is used to determine the maximum beam length</remarks>
		/// <param name="referenceHeight">Height of the drawing area / background image</param>
		/// <param name="redraw">Delegate that redraws the preview</param>
		void Initialize(int referenceHeight, Action redraw);

		/// <summary>
		/// Gets the OpenGL moving head associated with the shape.
		/// </summary>
		/// <remarks>This property allows all the moving heads to drawn together to improve performance</remarks>
		IRenderMovingHeadOpenGL MovingHead { get; }

		/// <summary>
		/// Updates the volumes for the specified maximum beam length and scale factor.
		/// </summary>
		/// <param name="maxBeamLength">Maximum beam length</param>		
		/// <param name="referenceHeight">Height of the background</param>
		void UpdateVolumes(int maxBeamLength, int referenceHeight);
	}
}
