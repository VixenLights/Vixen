using OpenTK.Mathematics;

namespace Common.OpenGLCommon.Constructs.DrawingEngine
{
	/// <summary>
	/// Helper class to determine if the 2-D mouse cursor is over a 3-D OpenGL shape.
	/// </summary>
	/// <remarks>Some of the algorithms in this class were created with the help of AI</remarks>
	public static class MouseHitTest
	{
		#region Public Methods

		/// <summary>
		/// Creates a ray from the 2-D mouse position.
		/// </summary>
		/// <param name="mousePos">Position of the mouse in 2-D screen coordinates</param>
		/// <param name="projection">OpenGL projection matrix</param>
		/// <param name="view">OpenGL view matrix</param>
		/// <param name="screenWidth">Width of the OpenTK control</param>
		/// <param name="screenHeight">Height of the OpenTK control</param>
		/// <returns>3-D direction vector of the mouse click</returns>
		public static Vector3 CreateRayFromMouse(
			Vector2 mousePos,
			Matrix4 projection,
			Matrix4 view,
			int screenWidth,
			int screenHeight)
		{
			// Convert to Normalized Device Coordinates (NDC)
			float x = (2.0f * mousePos.X) / screenWidth - 1.0f;
			float y = 1.0f - (2.0f * mousePos.Y) / screenHeight; // invert Y
			Vector4 clipCoords = new Vector4(x, y, -1.0f, 1.0f);

			// Convert to eye space
			Matrix4 invProj = projection.Inverted();
			Vector4 eyeCoords = invProj * clipCoords;
			eyeCoords.Z = -1.0f;
			eyeCoords.W = 0.0f;

			// Convert to world space
			Matrix4 invView = view.Inverted();
			Vector4 worldCoords = invView * eyeCoords;

			Vector3 rayDirection = Vector3.Normalize(new Vector3(worldCoords.X, worldCoords.Y, worldCoords.Z));
			Vector3 rayOrigin = invView.ExtractTranslation();
			
			return rayDirection;
		}

		/// <summary>
		/// Returns true if the specified ray intersects an axis aligned bounding box.
		/// </summary>
		/// <param name="rayOrigin">Origin of the ray</param>
		/// <param name="rayDir">Direction of the ray</param>
		/// <param name="min">Minimum vector of the bounding box</param>
		/// <param name="max">Maximum vector of the bounding box</param>
		/// <param name="distance">Distance along the ray to the first intersection point</param>
		/// <returns>True if the ray intersects the bounding box</returns>
		public static bool RayIntersectsAABB(
			Vector3 rayOrigin, 
			Vector3 rayDir,
			Vector3 min, 
			Vector3 max,
			out float distance)
		{
			distance = 0f;

			float tmin = float.NegativeInfinity;
			float tmax = float.PositiveInfinity;

			// For each axis
			for (int i = 0; i < 3; i++)
			{
				float origin = rayOrigin[i];
				float dir = rayDir[i];
				float minB = min[i];
				float maxB = max[i];

				if (MathF.Abs(dir) < 1e-6f)
				{
					// Ray is parallel to this axis
					if (origin < minB || origin > maxB)
						return false; // Outside slab
				}
				else
				{
					float t1 = (minB - origin) / dir;
					float t2 = (maxB - origin) / dir;

					if (t1 > t2) (t1, t2) = (t2, t1);

					tmin = MathF.Max(tmin, t1);
					tmax = MathF.Min(tmax, t2);

					if (tmin > tmax)
						return false;
				}
			}

			distance = tmin >= 0 ? tmin : tmax;
			return distance >= 0;
		}

		#endregion
	}
}
