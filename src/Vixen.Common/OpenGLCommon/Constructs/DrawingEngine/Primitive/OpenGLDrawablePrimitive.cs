using Common.OpenGLCommon.Constructs.Vertex;

using OpenTK.Mathematics;

namespace Common.OpenGLCommon.Constructs.DrawingEngine.Primitive
{
	/// <summary>
	/// Maintains OpenGL state associated with a drawing primitive.
	/// </summary>
	public class OpenGLDrawablePrimitive : IOpenGLDrawablePrimitive
	{
		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		public OpenGLDrawablePrimitive()
		{
			// Default the vertext array object to invalid
			VAO = -1;

			// Create the collection of vertices
			Vertices = new();

			CalculateMinAndMaxFromVertices = true;
		}

		#endregion

		#region IOpenGLDrawablePrimitive

		/// <inheritdoc/>
		public VBO<float> VBO { get; set; }

		/// <inheritdoc/>
		public int VAO { get; set; }

		/// <inheritdoc/>
		public List<float> Vertices { get; set; }

		/// <inheritdoc/>
		public int BufferSize { get; set; }

		/// <inheritdoc/>
		public virtual Vector3 GetMinimum()
		{
			if (CalculateMinAndMaxFromVertices)
			{
				float minX = float.PositiveInfinity;
				float minY = float.PositiveInfinity;
				float minZ = float.PositiveInfinity;

				for (int i = 0; i < Vertices.Count; i += 3)
				{
					if (Vertices[i] < minX) minX = Vertices[i];

					if (Vertices[i + 1] < minY) minY = Vertices[i + 1];

					if (Vertices[i + 2] < minZ) minZ = Vertices[i + 2];
				}
				Minimum = new Vector3(minX, minY, minZ);
			}

			return Minimum;
		}

		/// <inheritdoc/>
		public virtual Vector3 GetMaximum()
		{
			if (CalculateMinAndMaxFromVertices)
			{
				float maxX = float.NegativeInfinity;
				float maxY = float.NegativeInfinity;
				float maxZ = float.NegativeInfinity;

				for (int i = 0; i < Vertices.Count; i += 3)
				{
					if (Vertices[i] > maxX) maxX = Vertices[i];

					if (Vertices[i + 1] > maxY) maxY = Vertices[i + 1];

					if (Vertices[i + 2] > maxZ) maxZ = Vertices[i + 2];
				}
				Maximum = new Vector3(maxX, maxY, maxZ);
			}

			return Maximum;
		}

		/// <inheritdoc/>
		public bool MouseOver(Matrix4 projectionMatrix, Matrix4 viewMatrix, int width, int height, Vector2 mousePos)
		{
			bool mouseOver = false;


			var (rayOrigin, rayDir) = MouseHitTest.CreateRayFromMouse(
							mousePos,
							projectionMatrix,
							viewMatrix,
							width,
							height);

			Vector3 min = GetMinimum();
			Vector3 max = GetMaximum();

			float distance = 0;
			if (MouseHitTest.RayIntersectsAABB(rayOrigin, rayDir, min, max, out distance))
			{
				mouseOver = true;
			}

			return mouseOver;
		}

		#endregion

		#region Protected Properties

		protected Vector3 Minimum { get; set; }

		protected Vector3 Maximum { get; set; }

		protected bool CalculateMinAndMaxFromVertices { get; set; }

		#endregion

		#region IDisposable

		/// <inheritdoc/>
		public void Dispose()
		{
			// If the vertex buffer object has been initialized then...
			if (VBO != null)
			{
				// Dispose of the vertex buffer object
				VBO.Dispose();
				VBO = null;
			}
		}

		#endregion
	}
}
