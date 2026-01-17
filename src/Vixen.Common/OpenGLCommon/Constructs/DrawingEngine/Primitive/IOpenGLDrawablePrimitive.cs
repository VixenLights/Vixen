using Common.OpenGLCommon.Constructs.Vertex;

using OpenTK.Mathematics;

namespace Common.OpenGLCommon.Constructs.DrawingEngine.Primitive
{
	/// <summary>
	/// Maintains OpenGL properties to draw a primitive.
	/// </summary>
	public interface IOpenGLDrawablePrimitive : IDisposable
	{
		/// <summary>
		/// Vertex array object.
		/// </summary>
		int VAO { get; set; }

		/// <summary>
		/// Vertex buffer object associated with the primitive.
		/// </summary>
		VBO<float> VBO { get; set; }

		/// <summary>
		/// VBO buffer size.
		/// </summary>
		int BufferSize { get; set; }

		/// <summary>
		/// Vertices associated with the primitive.
		/// </summary>
		List<float> Vertices { get; set; }

		/// <summary>
		/// Gets the minimum vertex of the primitive taking into account all axis.
		/// </summary>
		/// <returns>Minimum vertex of the primitive</returns>
		Vector3 GetMinimum();

		/// <summary>
		/// Gets the maximum vertex of the primitive taking into account all axis.
		/// </summary>
		/// <returns>Maximum vertex of the primitive</returns>
		Vector3 GetMaximum();

		/// <summary>
		/// Returns true if the mouse is over the primitive.
		/// </summary>
		/// <param name="projectionMatrix">OpenGL projection matrix</param>
		/// <param name="viewMatrix">OpenGL view (camera matrix)</param>
		/// <param name="width">Width of the OpenTKControl</param>
		/// <param name="height">Height of the OpenTKControl</param>
		/// <param name="mousePosition">Mouse position in screen coordinates</param>
		/// <returns>True if the mouse is over the primitive</returns>
		bool MouseOver(Matrix4 projectionMatrix, Matrix4 viewMatrix, int width, int height, Vector2 mousePosition);
	}
}
