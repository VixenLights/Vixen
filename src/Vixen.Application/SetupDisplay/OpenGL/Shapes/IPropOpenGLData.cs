
using Common.OpenGLCommon.Constructs.DrawingEngine.Primitive;
using Common.OpenGLCommon.Constructs.DrawingEngine.Shape;

using OpenTK.Mathematics;

namespace VixenApplication.SetupDisplay.OpenGL.Shapes
{
	/// <summary>
	/// Maintains OpenGL state for drawing props using OpenGL.
	/// </summary>
	public interface IPropOpenGLData : IOpenGLSelectableProp, IOpenGLDrawablePrimitive, IDisposable
	{
		/// <summary>
		/// X axis position of the prop.
		/// </summary>
		float X { get; set; }
		
		/// <summary>
		/// Y axis position of the prop.
		/// </summary>
		float Y { get; set; }

		/// <summary>
		/// Size of the prop along the X axis.
		/// </summary>
		public float SizeX { get; set; }
		
		/// <summary>
		/// Size of the prop along the Y axis.
		/// </summary>
		public float SizeY { get; set; }

		/// <summary>
		/// Size of the prop along the Z axis.
		/// </summary>
		public float SizeZ { get; set; }

		/// <summary>
		/// Indicates if the prop is selected in the preview.
		/// </summary>
		bool Selected { get; set; }

		/// <summary>
		/// Gets the minimum vertex values along all three axis.
		/// </summary>
		/// <returns>Minimum vertex values along all three axis</returns>
		Vector3 GetMinimum();

		/// <summary>
		/// Gets the maximum vertex values along all three axis.
		/// </summary>
		/// <returns>Maximum vertex values along all three axis</returns>
		Vector3 GetMaximum();

		/// <summary>
		/// Returns true if the mouse if over a resize handle.
		/// </summary>
		/// <param name="viewMatrix">View matrix of the viewport</param>
		/// <param name="projectionMatrix">Projection matrix of the viewport</param>
		/// <param name="width">Width of the OpenTK control</param>
		/// <param name="height">Height of the OpenTK control</param>
		/// <param name="mousePos">Position of the mouse in screen coordinates</param>
		/// <param name="handle">Handle the mouse is over</param>
		/// <returns>True if the mouse is over a resize handle</returns>
		bool MouseOverResizeHandle(
			Matrix4 viewMatrix, 
			Matrix4 projectionMatrix, 
			int width, 
			int height, 
			Vector2 mousePos, 
			out ResizeHandles handle);
	}
}
