using System.Runtime.InteropServices;

using Common.OpenGLCommon.Constructs;
using Common.OpenGLCommon.Constructs.DrawingEngine.Shape;

using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

using Common.OpenGLCommon.Constructs.Shaders;
using Common.OpenGLCommon.Constructs.Vertex;
using Common.OpenGLCommon.Constructs.DrawingEngine.Primitive;

namespace Common.OpenGLCommon
{
	/// <summary>
	/// Utility for drawing light points that make up a prop or shape.
	/// </summary>
	public static class DrawPointsUtility
	{		
		private static int EightFloatDataSize = 8 * Marshal.SizeOf(typeof(float));
		
		/// <summary>
		/// Draws the points that make up the light based shape.
		/// </summary>		
		public static void DrawPoints(IOpenGLLightBasedDrawable prop)
		{
			//Logging.Debug("Entering Draw.");
			if (prop.GetPoints().Count == 0)
			{
				//Logging.Debug("Exiting Draw.");
				return;
			}

			// If the points Vertex Buffer Object has not been created then
			// this is the first time drawing this shape and additional configuration is required.
			if (prop.VertexBufferObject == null)
			{
				// Create the Vertex Array Object 
				GL.GenVertexArrays(1, out int vao);
				prop.VAO = vao;

				// Bind the Vertex Array Object
				// Any Vertex Array configuration will be associated with this VAO.
				GL.BindVertexArray(prop.VAO);

				// Create the Vertex Buffer Object passing the light points
				prop.VertexBufferObject = new VBO<float>(prop.GetPoints().ToArray());

				// Tell OpenGL/OpenTK which buffer we want to work with for subsequent operations
				GlUtility.BindBuffer<float>((VBO<float>)prop.VertexBufferObject);

				// Store off the size of the buffer
				prop.PointsBufferSize = ((VBO<float>)prop.VertexBufferObject).Count;

				// Specify the format and location of the vertex attribute data in the VBO
				GL.VertexAttribPointer(ShaderProgram.VertexPosition, 3, VertexAttribPointerType.Float, false, EightFloatDataSize, IntPtr.Zero);
				GL.EnableVertexAttribArray(ShaderProgram.VertexPosition);
				GL.VertexAttribPointer(ShaderProgram.VertexColor, 4, VertexAttribPointerType.Float, false, EightFloatDataSize, Vector3.SizeInBytes);
				GL.EnableVertexAttribArray(ShaderProgram.VertexColor);
				GL.VertexAttribPointer(ShaderProgram.VertexSize, 1, VertexAttribPointerType.Float, false, EightFloatDataSize, Vector3.SizeInBytes + Vector4.SizeInBytes);
				GL.EnableVertexAttribArray(ShaderProgram.VertexSize);

				GL.DisableVertexAttribArray(ShaderProgram.TextureCoords);
			}
			else
			{
				// Bind the Vertex Array Object
				GL.BindVertexArray(prop.VAO);

				// Tell OpenGL/OpenTK which buffer we want to work with for subsequent operations
				GlUtility.BindBuffer<float>((VBO<float>)prop.VertexBufferObject);

				// If the buffer is already of sufficient size then...
				if (prop.PointsBufferSize >= prop.GetPoints().Count)
				{
					// Update the point data in the GPU
					GL.BufferSubData<float>(BufferTarget.ArrayBuffer, 0, prop.GetPoints().Count * sizeof(float), prop.GetPoints().ToArray());
				}
				else
				{
					// Create and initialize a buffer object's data store. Allocate memory for the buffer and initialize the buffer with data
					// This only needs to done when increasing the size of the buffer
					GL.BufferData<float>(BufferTarget.ArrayBuffer, prop.GetPoints().Count * sizeof(float), prop.GetPoints().ToArray(), BufferUsageHint.StreamDraw);

					// Update the size of the buffer
					prop.PointsBufferSize = prop.GetPoints().Count;
				}
			}

			// Draw the points
			GL.DrawArrays(PrimitiveType.Points, 0, prop.GetPoints().Count / 8);
		}

		/// <summary>
		/// Draws the points that make up the OpenGL primitive.
		/// </summary>		
		public static void DrawPoints(IOpenGLDrawablePrimitive prop)
		{
			//Logging.Debug("Entering Draw.");
			if (prop.Vertices.Count == 0)
			{
				//Logging.Debug("Exiting Draw.");
				return;
			}

			// If the points Vertex Buffer Object has not been created then
			// this is the first time drawing this shape and additional configuration is required.
			if (prop.VBO == null)
			{
				// Create the Vertex Array Object 
				GL.GenVertexArrays(1, out int vao);
				prop.VAO = vao;

				// Bind the Vertex Array Object
				// Any Vertex Array configuration will be associated with this VAO.
				GL.BindVertexArray(prop.VAO);

				// Create the Vertex Buffer Object passing the light points
				prop.VBO = new VBO<float>(prop.Vertices.ToArray());

				// Tell OpenGL/OpenTK which buffer we want to work with for subsequent operations
				GlUtility.BindBuffer<float>((VBO<float>)prop.VBO);

				// Store off the size of the buffer
				prop.BufferSize = ((VBO<float>)prop.VBO).Count;

				// Specify the format and location of the vertex attribute data in the VBO
				GL.VertexAttribPointer(ShaderProgram.VertexPosition, 3, VertexAttribPointerType.Float, false, EightFloatDataSize, IntPtr.Zero);
				GL.EnableVertexAttribArray(ShaderProgram.VertexPosition);
				GL.VertexAttribPointer(ShaderProgram.VertexColor, 4, VertexAttribPointerType.Float, false, EightFloatDataSize, Vector3.SizeInBytes);
				GL.EnableVertexAttribArray(ShaderProgram.VertexColor);
				GL.VertexAttribPointer(ShaderProgram.VertexSize, 1, VertexAttribPointerType.Float, false, EightFloatDataSize, Vector3.SizeInBytes + Vector4.SizeInBytes);
				GL.EnableVertexAttribArray(ShaderProgram.VertexSize);

				GL.DisableVertexAttribArray(ShaderProgram.TextureCoords);
			}
			else
			{
				// Bind the Vertex Array Object
				GL.BindVertexArray(prop.VAO);

				// Tell OpenGL/OpenTK which buffer we want to work with for subsequent operations
				GlUtility.BindBuffer<float>((VBO<float>)prop.VBO);

				// If the buffer is already of sufficient size then...
				if (prop.BufferSize >= prop.Vertices.Count)
				{
					// Update the point data in the GPU
					GL.BufferSubData<float>(BufferTarget.ArrayBuffer, 0, prop.Vertices.Count * sizeof(float), prop.Vertices.ToArray());
				}
				else
				{
					// Create and initialize a buffer object's data store. Allocate memory for the buffer and initialize the buffer with data
					// This only needs to done when increasing the size of the buffer
					GL.BufferData<float>(BufferTarget.ArrayBuffer, prop.Vertices.Count * sizeof(float), prop.Vertices.ToArray(), BufferUsageHint.StreamDraw);

					// Update the size of the buffer
					prop.BufferSize = prop.Vertices.Count;
				}
			}

			// Draw the points
			GL.DrawArrays(PrimitiveType.Points, 0, prop.Vertices.Count / 8);
		}


	}
}
