using System;
using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL;
using VixenModules.Preview.VixenPreview.OpenGL.Constructs.Shaders;
using VixenModules.Preview.VixenPreview.OpenGL.Constructs.Vertex;

namespace VixenModules.Preview.VixenPreview.OpenGL.Constructs
{
	public class GlUtility
	{

		private static readonly uint[] Uint1 = new uint[1];
		
		/// <summary>
		/// Creates and initializes a buffer object's data store.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="target">Specifies the target buffer object.</param>
		/// <param name="size">Specifies the size in bytes of the buffer object's new data store.</param>
		/// <param name="data">Specifies a pointer to data that will be copied into the data store for initialization, or NULL if no data is to be copied.</param>
		/// <param name="usage">Specifies expected usage pattern of the data store.</param>
		public static void BufferData<T>(BufferTarget target, Int32 size, [In, Out] T[] data, BufferUsageHint usage)
			where T : struct
		{
			GCHandle dataPtr = GCHandle.Alloc(data, GCHandleType.Pinned);
			try
			{
				GL.BufferData(target, new IntPtr(size), dataPtr.AddrOfPinnedObject(), usage);
			}
			finally
			{
				dataPtr.Free();
			}
		}

		/// <summary>
		/// Creates and initializes a buffer object's data store.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="target">Specifies the target buffer object.</param>
		/// <param name="size">Specifies the size in bytes of the buffer object's new data store.</param>
		/// <param name="data">Specifies a pointer to data that will be copied into the data store for initialization, or NULL if no data is to be copied.</param>
		/// <param name="usage">Specifies expected usage pattern of the data store.</param>
		public static void BufferData<T>(BufferTarget target, Int32 position, Int32 size, [In, Out] T[] data, BufferUsageHint usage)
			where T : struct
		{
			GCHandle dataPtr = GCHandle.Alloc(data, GCHandleType.Pinned);
			try
			{
				GL.BufferData(target, new IntPtr(size), (IntPtr)((int)dataPtr.AddrOfPinnedObject() + position), usage);
			}
			finally
			{
				dataPtr.Free();
			}
		}

		/// <summary>
		/// Shortcut for quickly generating a single buffer id without creating an array to
		/// pass to the gl function.  Calls Gl.GenBuffers(1, id).
		/// </summary>
		/// <returns>The ID of the generated buffer.  0 on failure.</returns>
		public static uint GenBuffer()
		{
			Uint1[0] = 0;
			GL.GenBuffers(1, Uint1);
			return Uint1[0];
		}

		/// <summary>
		/// Shortcut for quickly generating a single texture id without creating an array to
		/// pass to the gl function.  Calls Gl.GenTexture(1, id).
		/// </summary>
		/// <returns>The ID of the generated texture.  0 on failure.</returns>
		public static uint GenTexture()
		{
			Uint1[0] = 0;
			GL.GenTextures(1, Uint1);
			return Uint1[0];
		}

		/// <summary>
		/// Bind a named texture to a texturing target
		/// </summary>
		/// <param name="Texture">Specifies the texture.</param>
		public static void BindTexture(Texture Texture)
		{
			GL.BindTexture(Texture.TextureTarget, Texture.TextureID);
		}

		/// <summary>
		/// Creates a standard VBO of type T.
		/// </summary>
		/// <typeparam name="T">The type of the data being stored in the VBO (make sure it's byte aligned).</typeparam>
		/// <param name="target">The VBO BufferTarget (usually ArrayBuffer or ElementArrayBuffer).</param>
		/// <param name="data">The data to store in the VBO.</param>
		/// <param name="hint">The buffer usage hint (usually StaticDraw).</param>
		/// <returns>The buffer ID of the VBO on success, 0 on failure.</returns>
		public static uint CreateVBO<T>(BufferTarget target, [In, Out] T[] data, BufferUsageHint hint)
			where T : struct
		{
			uint vboHandle = GenBuffer();
			if (vboHandle == 0) return 0;

			int size = data.Length * Marshal.SizeOf(typeof(T));

			GL.BindBuffer(target, vboHandle);
			GL.BufferData<T>(target, size, data, hint);
			GL.BindBuffer(target, 0);
			return vboHandle;
		}

		/// <summary>
		/// Creates a standard VBO of type T where the length of the VBO is less than or equal to the length of the data.
		/// </summary>
		/// <typeparam name="T">The type of the data being stored in the VBO (make sure it's byte aligned).</typeparam>
		/// <param name="target">The VBO BufferTarget (usually ArrayBuffer or ElementArrayBuffer).</param>
		/// <param name="data">The data to store in the VBO.</param>
		/// <param name="hint">The buffer usage hint (usually StaticDraw).</param>
		/// <param name="length">The length of the VBO (will take the first 'length' elements from data).</param>
		/// <returns>The buffer ID of the VBO on success, 0 on failure.</returns>
		public static uint CreateVBO<T>(BufferTarget target, [In, Out] T[] data, BufferUsageHint hint, int length)
			where T : struct
		{
			uint vboHandle = GenBuffer();
			if (vboHandle == 0) return 0;

			int size = length * Marshal.SizeOf(typeof(T));
		
			GL.BindBuffer(target, vboHandle);
			GL.BufferData<T>(target, size, data, hint);
			GL.BindBuffer(target, 0);
			return vboHandle;
		}

		/// <summary>
		/// Creates a standard VBO of type T where the length of the VBO is less than or equal to the length of the data.
		/// </summary>
		/// <typeparam name="T">The type of the data being stored in the VBO (make sure it's byte aligned).</typeparam>
		/// <param name="target">The VBO BufferTarget (usually ArrayBuffer or ElementArrayBuffer).</param>
		/// <param name="data">The data to store in the VBO.</param>
		/// <param name="hint">The buffer usage hint (usually StaticDraw).</param>
		/// <param name="position"></param>
		/// <param name="length">The length of the VBO (will take the first 'length' elements from data).</param>
		/// <returns>The buffer ID of the VBO on success, 0 on failure.</returns>
		public static uint CreateVBO<T>(BufferTarget target, [In, Out] T[] data, BufferUsageHint hint, int position, int length)
			where T : struct
		{
			uint vboHandle = GenBuffer();
			if (vboHandle == 0) return 0;

			int offset = position * Marshal.SizeOf(typeof(T));
			int size = length * Marshal.SizeOf(typeof(T));

			GL.BindBuffer(target, vboHandle);
			BufferData<T>(target, offset, size, data, hint);
			GL.BindBuffer(target, 0);
			return vboHandle;
		}

		public static void BindBuffer<T>(VBO<T> buffer) where T : struct
		{
			GL.BindBuffer(buffer.BufferTarget, buffer.vboID);
		}

		public static void BindBufferToShaderAttribute<T>(VBO<T> buffer, ShaderProgram program, string attributeName) where T : struct
		{
			uint attribLocation = (uint)GL.GetAttribLocation(program.ProgramId, attributeName);
			GL.EnableVertexAttribArray(attribLocation);
			BindBuffer<T>(buffer);
			GL.VertexAttribPointer(attribLocation, buffer.Size, buffer.PointerType, true, Marshal.SizeOf(typeof(T)), IntPtr.Zero);
		}

	}
}