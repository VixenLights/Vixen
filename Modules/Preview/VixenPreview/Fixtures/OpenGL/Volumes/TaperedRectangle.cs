using OpenTK;
using System.Collections.Generic;

namespace VixenModules.Preview.VixenPreview.Fixtures.OpenGL.Volumes
{
	/// <summary>
	/// Defines a 3-D Tapered Rectangle.
	/// </summary>
	public class TaperedRectangle : VolumeBase
	{		
		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="width">Width of the 3-D rectangle</param>
		/// <param name="height">Height of the 3-D rectangle</param>
		/// <param name="baseDepth">Base depth of the 3-D rectangle</param>
		/// <param name="topDepth">Top depth of the 3-D rectangle</param>
		/// <param name="isDynamic">Whether the volume change geometry during execution</param>
		public TaperedRectangle(float width, float height, float baseDepth, float topDepth, bool isDynamic) : base(isDynamic)
		{			
			// Create the vertices of the tapered 3-D rectangle
			CreateTaperedRectangleVertices(width, height, baseDepth, topDepth, Vertices);

			// Create the normals for the tapered rectangle vertices
			CreateRectangleNormals(Normals);
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Creates the tapered rectangle vertices based on the specified dimensions.
		/// </summary>
		/// <param name="width">Width of the rectangle</param>
		/// <param name="height">Height of the rectangle</param>
		/// <param name="baseDepth">Base depth of the 3-D rectangle</param>
		/// <param name="topDepth">Top depth of the 3-D rectangle</param>		
		/// <param name="vertices">Vertices collection to populate</param>
		private void CreateTaperedRectangleVertices(float width, float height, float baseDepth, float topDepth, List<Vector3> vertices)
		{
			// Create vertices for the Front face
			vertices.Add(new Vector3(-width, -height, baseDepth)); // 0
			vertices.Add(new Vector3(width, -height, baseDepth));  // 1
			vertices.Add(new Vector3(width, height, topDepth));    // 2

			vertices.Add(new Vector3(-width, -height, baseDepth)); // 0
			vertices.Add(new Vector3(width, height, topDepth));    // 2
			vertices.Add(new Vector3(-width, height, topDepth));   // 3

			// Create vertices for the Back face
			vertices.Add(new Vector3(-width, -height, -baseDepth)); // 0 
			vertices.Add(new Vector3(width, -height, -baseDepth));  // 1
			vertices.Add(new Vector3(width, height, -topDepth));    // 2

			vertices.Add(new Vector3(-width, -height, -baseDepth)); // 0
			vertices.Add(new Vector3(width, height, -topDepth));    // 2
			vertices.Add(new Vector3(-width, height, -topDepth));   // 3

			// Create vertices for the Top face
			vertices.Add(new Vector3(width, height, topDepth));   // 0
			vertices.Add(new Vector3(width, height, -topDepth));  // 1
			vertices.Add(new Vector3(-width, height, -topDepth)); // 2

			vertices.Add(new Vector3(width, height, topDepth));   // 0
			vertices.Add(new Vector3(-width, height, -topDepth)); // 2
			vertices.Add(new Vector3(-width, height, topDepth));  // 3

			// Create vertices for the Bottom face
			vertices.Add(new Vector3(width, -height, baseDepth));   // 0
			vertices.Add(new Vector3(width, -height, -baseDepth));  // 1
			vertices.Add(new Vector3(-width, -height, -baseDepth)); // 2

			vertices.Add(new Vector3(width, -height, baseDepth));   // 0
			vertices.Add(new Vector3(-width, -height, -baseDepth)); // 2
			vertices.Add(new Vector3(-width, -height, baseDepth));  // 3

			// Create vertices for the Right face
			vertices.Add(new Vector3(width, -height, baseDepth));  // 0
			vertices.Add(new Vector3(width, -height, -baseDepth)); // 1
			vertices.Add(new Vector3(width, height, -topDepth));   // 2

			vertices.Add(new Vector3(width, -height, baseDepth)); // 0
			vertices.Add(new Vector3(width, height, -topDepth));  // 2
			vertices.Add(new Vector3(width, height, topDepth));   // 3

			// Create vertices for the Left face
			vertices.Add(new Vector3(-width, -height, baseDepth));  // 0
			vertices.Add(new Vector3(-width, -height, -baseDepth)); // 1
			vertices.Add(new Vector3(-width, height, -topDepth));   // 2

			vertices.Add(new Vector3(-width, -height, baseDepth)); // 0
			vertices.Add(new Vector3(-width, height, -topDepth));  // 2
			vertices.Add(new Vector3(-width, height, topDepth));   // 3
		}

		/// <summary>
		/// Creates the normals for the tapered rectangle vertices.
		/// </summary>
		/// <param name="normals">Collection of normal to populate</param>
		private void CreateRectangleNormals(List<Vector3> normals, bool negateNormals = false)
		{
			{
				Vector3 v0 = Vertices[0];
				Vector3 v1 = Vertices[1];
				Vector3 v2 = Vertices[2];

				// Set the normal vector for lighting the Front face
				if (!negateNormals)
				{
					// Set the normal vector for the first point
					normals.Add(Vector3.Cross(v0 - v1, v0 - v2).Normalized());
					normals.Add(Vector3.Cross(v0 - v1, v0 - v2).Normalized());
					normals.Add(Vector3.Cross(v0 - v1, v0 - v2).Normalized());
					normals.Add(Vector3.Cross(v0 - v1, v0 - v2).Normalized());
					normals.Add(Vector3.Cross(v0 - v1, v0 - v2).Normalized());
					normals.Add(Vector3.Cross(v0 - v1, v0 - v2).Normalized());
				}
				else
				{
					// Set the normal vector for the first point
					normals.Add(-1 * Vector3.Cross(v0 - v1, v0 - v2).Normalized());
					normals.Add(-1 * Vector3.Cross(v0 - v1, v0 - v2).Normalized());
					normals.Add(-1 * Vector3.Cross(v0 - v1, v0 - v2).Normalized());
					normals.Add(-1 * Vector3.Cross(v0 - v1, v0 - v2).Normalized());
					normals.Add(-1 * Vector3.Cross(v0 - v1, v0 - v2).Normalized());
					normals.Add(-1 * Vector3.Cross(v0 - v1, v0 - v2).Normalized());
				}
			}

			{
				Vector3 v0 = Vertices[6];
				Vector3 v1 = Vertices[7];
				Vector3 v2 = Vertices[8];

				// Set the normal vector for lighting the Back face
				if (negateNormals)
				{
					// Set the normal vector for the first point
					normals.Add(Vector3.Cross(v0 - v1, v0 - v2).Normalized());
					normals.Add(Vector3.Cross(v0 - v1, v0 - v2).Normalized());
					normals.Add(Vector3.Cross(v0 - v1, v0 - v2).Normalized());
					normals.Add(Vector3.Cross(v0 - v1, v0 - v2).Normalized());
					normals.Add(Vector3.Cross(v0 - v1, v0 - v2).Normalized());
					normals.Add(Vector3.Cross(v0 - v1, v0 - v2).Normalized());
				}
				else
				{
					// Set the normal vector for the first point
					normals.Add(-1 * Vector3.Cross(v0 - v1, v0 - v2).Normalized());
					normals.Add(-1 * Vector3.Cross(v0 - v1, v0 - v2).Normalized());
					normals.Add(-1 * Vector3.Cross(v0 - v1, v0 - v2).Normalized());
					normals.Add(-1 * Vector3.Cross(v0 - v1, v0 - v2).Normalized());
					normals.Add(-1 * Vector3.Cross(v0 - v1, v0 - v2).Normalized());
					normals.Add(-1 * Vector3.Cross(v0 - v1, v0 - v2).Normalized());
				}
			}

			// Set the normal vector for lighting the Top face
			normals.Add(new Vector3(0, 1, 0));
			normals.Add(new Vector3(0, 1, 0));
			normals.Add(new Vector3(0, 1, 0));
			normals.Add(new Vector3(0, 1, 0));
			normals.Add(new Vector3(0, 1, 0));
			normals.Add(new Vector3(0, 1, 0));

			// Set the normal vector for lighting the Bottom face
			normals.Add(new Vector3(0, -1, 0));
			normals.Add(new Vector3(0, -1, 0));
			normals.Add(new Vector3(0, -1, 0));
			normals.Add(new Vector3(0, -1, 0));
			normals.Add(new Vector3(0, -1, 0));
			normals.Add(new Vector3(0, -1, 0));

			// Set the normal vector for lighting the Right face
			normals.Add(new Vector3(1, 0, 0));
			normals.Add(new Vector3(1, 0, 0));
			normals.Add(new Vector3(1, 0, 0));
			normals.Add(new Vector3(1, 0, 0));
			normals.Add(new Vector3(1, 0, 0));
			normals.Add(new Vector3(1, 0, 0));

			// Set the normal vector for lighting the Left face
			normals.Add(new Vector3(-1, 0, 0));
			normals.Add(new Vector3(-1, 0, 0));
			normals.Add(new Vector3(-1, 0, 0));
			normals.Add(new Vector3(-1, 0, 0));
			normals.Add(new Vector3(-1, 0, 0));
			normals.Add(new Vector3(-1, 0, 0));
		}

		#endregion

		#region IVolume

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>
		public override void UpdateModelMatrix()
		{
			// Initialize the model matrix
			ModelMatrix = Matrix4.CreateRotationX(Rotation.X) *
			              Matrix4.CreateRotationY(Rotation.Y) *
			              Matrix4.CreateRotationZ(Rotation.Z) *
			              Matrix4.CreateTranslation(Position) *
			              Matrix4.CreateRotationX(GroupRotation.X) *
			              Matrix4.CreateRotationY(GroupRotation.Y) *
			              Matrix4.CreateRotationZ(GroupRotation.Z) *
			              Matrix4.CreateTranslation(GroupTranslation);
		}

		#endregion
	}
}
