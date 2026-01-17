using Common.OpenGLCommon.Constructs;
using Common.OpenGLCommon.Constructs.DrawingEngine.Primitive;
using Common.OpenGLCommon.Constructs.Shaders;
using Common.OpenGLCommon.Constructs.Vertex;

using OpenTK.Graphics.OpenGL;


namespace Common.OpenGLCommon
{
	/// <summary>
	/// Utility class for drawing open and closed lines.
	/// </summary>
	public static class DrawLineUtility
	{
		#region Public Static Methods

		/// <summary>
		/// Draws the points that make up the light based shape.
		/// </summary>		
		public static void DrawLines(IOpenGLDrawablePrimitive primitive)
		{
			// Configure the VAO and VBOs associated with the primitive
			ConfigurePrimitiveVAO(primitive);

			// Draw the lines
			GL.DrawArrays(PrimitiveType.Lines, 0, primitive.Vertices.Count / 2);
		}

		/// <summary>
		/// Draws the points that make up the light based shape.
		/// </summary>		
		public static void DrawLineLoop(IOpenGLDrawablePrimitive primitive)
		{
			// Configure the VAO and VBOs associated with the primitive
			ConfigurePrimitiveVAO(primitive);

			// Draw the line loops
			GL.DrawArrays(PrimitiveType.LineLoop, 0, primitive.Vertices.Count / 3);
		}

		#endregion 

		#region Private Methods

		/// <summary>
		/// Draws the points that make up the light based shape.
		/// </summary>		
		private static void ConfigurePrimitiveVAO(IOpenGLDrawablePrimitive primitive)
		{
			if (primitive.Vertices.Count == 0)
			{
				return;
			}

			// If the points Vertex Buffer Object has not been created then
			// this is the first time drawing this shape and additional configuration is required.
			if (primitive.VBO == null)
			{
				// Create the Vertex Array Object 
				GL.GenVertexArrays(1, out int vao);
				primitive.VAO = vao;

				// Bind the Vertex Array Object
				// Any Vertex Array configuration will be associated with this VAO.
				GL.BindVertexArray(primitive.VAO);

				// Create the Vertex Buffer Object passing the four corners of the prop
				primitive.VBO = new VBO<float>(primitive.Vertices.ToArray());

				// Tell OpenGL/OpenTK which buffer we want to work with for subsequent operations
				GlUtility.BindBuffer<float>((VBO<float>)primitive.VBO);

				// Store off the size of the buffer
				primitive.BufferSize = ((VBO<float>)primitive.VBO).Count;

				// Specify the format and location of the vertex attribute data in the VBO
				GL.VertexAttribPointer(ShaderProgram.VertexPosition, 3, VertexAttribPointerType.Float, false, 0, IntPtr.Zero);
				GL.EnableVertexAttribArray(ShaderProgram.VertexPosition);
			}
			else
			{
				// Bind the Vertex Array Object
				GL.BindVertexArray(primitive.VAO);

				// Tell OpenGL/OpenTK which buffer we want to work with for subsequent operations
				GlUtility.BindBuffer<float>((VBO<float>)primitive.VBO);

				// If the buffer is already of sufficient size then...
				if (primitive.BufferSize == primitive.Vertices.Count())
				{
					// Update the point data in the GPU
					GL.BufferSubData<float>(BufferTarget.ArrayBuffer, 0, primitive.Vertices.Count * sizeof(float), primitive.Vertices.ToArray());
				}
				else
				{
					// Create and initialize a buffer object's data store. Allocate memory for the buffer and initialize the buffer with data
					// This only needs to done when increasing the size of the buffer
					GL.BufferData<float>(BufferTarget.ArrayBuffer, primitive.Vertices.Count * sizeof(float), primitive.Vertices.ToArray(), BufferUsageHint.StreamDraw);

					// Update the size of the buffer
					primitive.BufferSize = primitive.Vertices.Count;
				}
			}			
		}

		#endregion
	}
}
