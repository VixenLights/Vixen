using OpenTK;

namespace VixenModules.Preview.VixenPreview.Shapes
{
	/// <summary>
	/// Interface to allow shapes to draw on the preview content that is not pixels or standard lights.
	/// </summary>
	public interface IDrawStaticPreviewShape
	{
		/// <summary>
		/// Initializes the GDI resources needed to draw the shape.
		/// </summary>
		void InitializeGDI();

		/// <summary>
		/// Draws the shape using the windows GDI.
		/// </summary>
		/// <param name="fp">Buffer to draw into</param>
		/// <param name="editMode">Whether the preview is being edited</param>		
		/// <param name="selected">Whether the shape is selected</param>
		/// <param name="zoomLevel">Zoom level of the preview</param>
		void DrawGDI(FastPixel.FastPixel fp, bool editMode, bool selected, double zoomLevel);
		
		/// <summary>
		/// Draws the shape using OpenGL.
		/// </summary>
		/// <param name="zDistance"></param>
		/// <param name="width">Width of the entire Vixen preview</param>
		/// <param name="height">Height of the entire Vixen preview</param>
		/// <param name="projectionMatrix">Projection matrix used for the OpenGL preview</param>
		/// <param name="viewMatrix">Maxtrix used to view the preview</param>
		/// <param name="scaleFactor">Scale factor of the OpenGL preview</param>
		/// <param name="referenceHeight">Height of the background</param>
		/// <param name="camera">Position of camera viewing preview</param>		
		void DrawOpenGL(float zDistance, int width, int height, Matrix4 projectionMatrix, Matrix4 viewMatrix, double scaleFactor, int referenceHeight, Vector3 camera);

		/// <summary>
		/// Disposes of OpenGL resources.
		/// </summary>
		/// <remarks>
		/// Not using IDipose here because the shapes lifetime is longer than the OpenGL preview lifetime.
		/// This method is called when closing the OpenGL preview and going to the GDI preview to edit the layout.
		/// </remarks>
		void DisposeOpenGLResources();
	}
}
