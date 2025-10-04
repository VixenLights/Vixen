using Common.OpenGLCommon.Constructs.Vertex;

namespace Common.OpenGLCommon.Constructs.DrawingEngine.Shape
{
	/// <summary>
	/// Maintains OpenGL data to draw light based props or shapes using OpenGL.
	/// </summary>
	public interface IOpenGLLightBasedDrawable
	{
		/// <summary>
		/// Vertex Buffer Object for the points that make up the light based shape.
		/// </summary>
		VBO<float> VertexBufferObject { get; set; }

		/// <summary>
		/// Vertex Array Object.
		/// </summary>
		int VAO { get; set; }

		/// <summary>
		/// Gets the vertex data needed to draw light points.
		/// </summary>
		/// <returns></returns>
		List<float> GetPoints();

		/// <summary>
		/// Gets the size of the vertext buffer object.
		/// </summary>
		int PointsBufferSize { get; set; }
	}
}
