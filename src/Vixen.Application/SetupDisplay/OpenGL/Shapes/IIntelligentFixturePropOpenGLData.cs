using VixenModules.Editor.FixtureGraphics.OpenGL;
using VixenApplication.SetupDisplay.OpenGL.Shapes;


namespace VixenApplication.SetupDisplay.OpenGL
{
	/// <summary>
	/// Interface to allow shapes to draw on the preview content that is not pixels or standard lights.
	/// </summary>
	public interface IIntelligentFixturePropOpenGLData : IPropOpenGLData
	{		
		/// <summary>
		/// Disposes of OpenGL resources.
		/// </summary>
		/// <remarks>
		/// Not using IDipose here because the shapes lifetime is longer than the OpenGL preview lifetime.
		/// This method is called when closing the OpenGL preview and going to the GDI preview to edit the layout.
		/// </remarks>
		void DisposeOpenGLResources();

		/// <summary>
		/// Initialize the moving head with the specified drawing area height.
		/// </summary>
		/// <remarks>The reference height is used to determine the maximum beam length</remarks>
		/// <param name="height">Height of the moving head</param>
		/// <param name="referenceHeight">Height of the drawing area / background image</param>
		/// <param name="redraw">Delegate that redraws the preview</param>
		void Initialize(float height, float referenceHeight, Action redraw);

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
		/// <param name="standardFrame">True when the volumes are being updated for standard frame update</param>
		void UpdateVolumes(int maxBeamLength, float referenceHeight, bool standardFrame);
	}
}
