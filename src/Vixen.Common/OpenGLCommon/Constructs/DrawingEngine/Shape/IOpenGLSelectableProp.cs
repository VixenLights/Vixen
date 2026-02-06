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
		/// Gets the left side drawing primitive for the selection cuboid.
		/// </summary>
		/// <returns>Drawing primitive for the left side of the selection cuboid</returns>
		IOpenGLDrawablePrimitive GetSelectionCuboidLeftSide();

		/// <summary>
		/// Gets the right side drawing primitive for the selection cuboid.
		/// </summary>
		/// <returns>Drawing primitive for the right side of the selection cuboid</returns>
		IOpenGLDrawablePrimitive GetSelectionCuboidRightSide();

		/// <summary>
		/// Gets the front side drawing primitive for the selection cuboid.
		/// </summary>
		/// <returns>Drawing primitive for the front side of the selection cuboid</returns>
		IOpenGLDrawablePrimitive GetSelectionCuboidFrontSide();

		/// <summary>
		/// Gets the front side drawing primitive for the selection cuboid.
		/// </summary>
		/// <returns>Drawing primitive for the back side of the selection cuboid</returns>
		IOpenGLDrawablePrimitive GetSelectionCuboidBackSide();
		
		/// <summary>
		/// Gets a center + drag move handle primitive geometry and OpenGL related state.
		/// </summary>
		/// <returns>Center + move handle primitive geometry and OpenGL related state</returns>
		IOpenGLDrawablePrimitive GetCenterX();				
	}
}
