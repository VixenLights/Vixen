using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;

#if USE_NUMERICS
using System.Numerics;
#endif

namespace VixenModules.Preview.VixenPreview.OpenGL.Constructs.Vertex
{
    public class VBO<T> : IDisposable
        where T : struct
    {
        #region Properties
        /// <summary>
        /// The ID of the vertex buffer object.
        /// </summary>
        public uint vboID { get; private set; }

        /// <summary>
        /// The type of the buffer.
        /// </summary>
        public BufferTarget BufferTarget { get; private set; }

        /// <summary>
        /// The size (in floats) of the type of data in the buffer.  Size * 4 to get bytes.
        /// </summary>
        public int Size { get; private set; }

        /// <summary>
        /// The type of data that is stored in the buffer (either int or float).
        /// </summary>
        public VertexAttribPointerType PointerType { get; private set; }
        
        /// <summary>
        /// The length of data that is stored in the buffer.
        /// </summary>
        public int Count { get; private set; }
        #endregion

        #region Constructor and Destructor
        /// <summary>
        /// Creates a buffer object of type T with a specified length.
        /// This allows the array T[] to be larger than the actual size necessary to buffer.
        /// Useful for reusing resources and avoiding unnecessary GC action.
        /// </summary>
        /// <param name="data">An array of data of type T (which must be a struct) that will be buffered to the GPU.</param>
        /// <param name="length">The length of the valid data in the data array.</param>
        /// <param name="target">Specifies the target buffer object.</param>
        /// <param name="hint">Specifies the expected usage of the data store.</param>
        public VBO(T[] data, int length, BufferTarget target = BufferTarget.ArrayBuffer, BufferUsageHint hint = BufferUsageHint.StaticDraw)
        {
            length = Math.Max(0, Math.Min(length, data.Length));

            vboID = GlUtility.CreateVBO<T>(BufferTarget = target, data, hint, length);

            this.Size = (data is int[] || data is float[] ? 1 : (data is Vector2[] ? 2 : (data is Vector3[] ? 3 : (data is Vector4[] ? 4 : 0))));
            this.PointerType = (data is int[] ? VertexAttribPointerType.Int : VertexAttribPointerType.Float);
            this.Count = length;
        }

	    /// <summary>
	    /// Creates a buffer object of type T with a specified length.
	    /// This allows the array T[] to be larger than the actual size necessary to buffer.
	    /// Useful for reusing resources and avoiding unnecessary GC action.
	    /// </summary>
	    /// <param name="data">An array of data of type T (which must be a struct) that will be buffered to the GPU.</param>
	    /// <param name="position"></param>
	    /// <param name="length">The length of the valid data in the data array.</param>
	    /// <param name="target">Specifies the target buffer object.</param>
	    /// <param name="hint">Specifies the expected usage of the data store.</param>
	    public VBO(T[] data, int position, int length, BufferTarget target = BufferTarget.ArrayBuffer, BufferUsageHint hint = BufferUsageHint.StaticDraw)
        {
            length = Math.Max(0, Math.Min(length, data.Length));

            vboID = GlUtility.CreateVBO<T>(BufferTarget = target, data, hint, position, length);

            this.Size = (data is int[] || data is float[] ? 1 : (data is Vector2[] ? 2 : (data is Vector3[] ? 3 : (data is Vector4[] ? 4 : 0))));
            this.PointerType = (data is int[] ? VertexAttribPointerType.Int : VertexAttribPointerType.Float);
            this.Count = length;
        }

        /// <summary>
        /// Creates a buffer object of type T.
        /// </summary>
        /// <param name="data">Specifies a pointer to data that will be copied into the data store for initialization.</param>
        /// <param name="target">Specifies the target buffer object.</param>
        /// <param name="hint">Specifies the expected usage of the data store.</param>
        public VBO(T[] data, BufferTarget target = BufferTarget.ArrayBuffer, BufferUsageHint hint = BufferUsageHint.StaticDraw)
        {
            vboID = GlUtility.CreateVBO<T>(BufferTarget = target, data, hint);

            Size = (data is int[] || data is float[] ? 1 : (data is Vector2[] ? 2 : (data is Vector3[] ? 3 : (data is Vector4[] ? 4 : 0))));
            PointerType = (data is int[] ? VertexAttribPointerType.Int : VertexAttribPointerType.Float);
            Count = data.Length;
        }

        /// <summary>
        /// Creates a static-read array buffer of type T.
        /// </summary>
        /// <param name="data">Specifies a pointer to data that will be copied into the data store for initialization.</param>
        public VBO(T[] data)
            : this(data, BufferTarget.ArrayBuffer, BufferUsageHint.StaticDraw)
        {
        }

        /// <summary>
        /// Check to ensure that the VBO was disposed of properly.
        /// </summary>
        ~VBO()
        {
            Dispose(false);
        }
        #endregion

 

        #region IDisposable
        /// <summary>
        /// Deletes this buffer from GPU memory.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (vboID != 0)
            {
                GL.DeleteBuffer(vboID);
                vboID = 0;
            }
        }
        #endregion
    }
}
