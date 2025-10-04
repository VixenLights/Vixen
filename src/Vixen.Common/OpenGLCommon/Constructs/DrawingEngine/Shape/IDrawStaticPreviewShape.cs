
using OpenTK.Mathematics;

namespace Common.OpenGLCommon.Constructs.DrawingEngine.Shape
{
	/// <summary>
	/// Interface to allow shapes to draw on the preview content that is not pixels or standard lights.
	/// </summary>
	public interface IDrawStaticPreviewShape
	{		
		/// <summary>
		/// Draws the shape using OpenGL.
		/// </summary>		
		/// <param name="projectionMatrix">Projection matrix used for the OpenGL preview</param>
		/// <param name="viewMatrix">Maxtrix used to view the preview</param>		
		/// <param name="referenceHeight">Height of the background</param>		
		void DrawOpenGL(			
			Matrix4 projectionMatrix,
			Matrix4 viewMatrix,
			int referenceHeight);			

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
