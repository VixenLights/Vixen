using Common.OpenGLCommon.Constructs.DrawingEngine.Primitive;

namespace Common.OpenGLCommon.Constructs.DrawingEngine.Shape
{
	/// <summary>
	/// Maintains state to aid with drawing indicators that a prop is selected in the preview.
	/// This state can be used to draw resize and move handles.
	/// </summary>
	public interface IOpenGLSelectableProp
	{		
		/// <summary>
		/// Gets the upper left resize box primitive geometry and OpenGL related state.
		/// </summary>
		/// <returns>Upper left resize box geometry and OpenGL related state</returns>
		IOpenGLDrawablePrimitive GetUpperLeftCornerResizeBox();

		/// <summary>
		/// Gets the upper right resize box primitive geometry and OpenGL related state.
		/// </summary>
		/// <returns>Upper right resize box geometry and OpenGL related state</returns>
		IOpenGLDrawablePrimitive GetUpperRightCornerResizeBox();

		/// <summary>
		/// Gets the lower right resize box primitive geometry and OpenGL related state.
		/// </summary>
		/// <returns>Lower right resize box geometry and OpenGL related state</returns>
		IOpenGLDrawablePrimitive GetLowerRightCornerResizeBox();

		/// <summary>
		/// Gets the lower left resize box primitive geometry and OpenGL related state.
		/// </summary>
		/// <returns>Lower left resize box geometry and OpenGL related state</returns>
		IOpenGLDrawablePrimitive GetLowerLeftCornerResizeBox();

		/// <summary>
		/// Gets the primitive geometry and OpenGL related state to draw a rectangle around a prop.
		/// </summary>
		/// <returns></returns>
		IOpenGLDrawablePrimitive GetSelectionRectangle();

		/// <summary>
		/// Gets a center + drag move handle primitive geometry and OpenGL related state.
		/// </summary>
		/// <returns>Center + move handle primitive geometry and OpenGL related state</returns>
		IOpenGLDrawablePrimitive GetCenterX();				
	}
}
