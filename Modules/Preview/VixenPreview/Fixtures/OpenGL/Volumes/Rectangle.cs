using OpenTK;
using System.Collections.Generic;

namespace VixenModules.Preview.VixenPreview.Fixtures.OpenGL.Volumes
{
	/// <summary>
	/// Defines a 3-D rectangle (rectangle cuboid).
	/// </summary>
	public class Rectangle : VolumeBase
	{
		#region Constructor 

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="width">Width of the 3-D rectangle</param>
		/// <param name="height">Height of the 3-D rectangle</param>
		/// <param name="depth">Depth of the 3-D rectangle</param>
		/// <param name="isDynamic">Whether the volume changes geomtery during execution</param>
		public Rectangle(float width, float height, float depth, bool isDynamic) : base(isDynamic)
		{
			// Create the rectangle vertices
			CreateRectangleVertices(width, height, depth, Vertices);

			// Create the rectangle normals
			CreateRectangleNormals(Normals);
		}

		#endregion

		#region Private Methods
			   		 
		/// <summary>
		/// Creates the rectangle vertices based on the specified dimensions.
		/// </summary>
		/// <param name="width">Width of the rectangle</param>
		/// <param name="height">Height of the rectangle</param>
		/// <param name="depth">Depth of the rectangle</param>
		/// <param name="vertices">Vertices collection to populate</param>
		private void CreateRectangleVertices(float width, float height, float depth, List<Vector3> vertices)
		{						
			// Create vertices for the Front face
			vertices.Add(new Vector3(-width, -height, depth)); // 0  Front Left Bottom Corner
			vertices.Add(new Vector3(width, -height, depth));  // 1  Front Right Bottom Corner
			vertices.Add(new Vector3(width, height, depth));   // 2  Front Right Top Corner

			vertices.Add(new Vector3(-width, -height, depth)); // 0 Front Left Bottom Corner
			vertices.Add(new Vector3(width, height, depth));   // 2 Front Right Top Corner
			vertices.Add(new Vector3(-width, height, depth));  // 3 Front Left Top Corner
			
			// Create vertices for the Back face
			vertices.Add(new Vector3(-width, -height, -depth)); // 4 Back Left Bottom Corner
			vertices.Add(new Vector3(width, -height, -depth));  // 5 Back Right Bottom Corner
			vertices.Add(new Vector3(width, height, -depth));   // 6 Back Right Top Corner

			vertices.Add(new Vector3(-width, -height, -depth)); // 4 
			vertices.Add(new Vector3(width, height, -depth));   // 5
			vertices.Add(new Vector3(-width, height, -depth));  // 7 Back Left Top Corner
			
			// Create vertices for the Top face
			vertices.Add(new Vector3(width, height, depth));   // 2
			vertices.Add(new Vector3(width, height, -depth));  // 6 
			vertices.Add(new Vector3(-width, height, -depth)); // 7

			vertices.Add(new Vector3(width, height, depth));   // 2
			vertices.Add(new Vector3(-width, height, -depth)); // 7
			vertices.Add(new Vector3(-width, height, depth));  // 3

			// Create vertices for the Bottom face
			vertices.Add(new Vector3(width, -height, depth));   // 1
			vertices.Add(new Vector3(width, -height, -depth));  // 5 
			vertices.Add(new Vector3(-width, -height, -depth)); // 4

			vertices.Add(new Vector3(width, -height, depth));   // 1
			vertices.Add(new Vector3(-width, -height, -depth)); // 4
			vertices.Add(new Vector3(-width, -height, depth));  // 0

			// Create vertices for the Right face
			vertices.Add(new Vector3(width, -height, depth));   // 1
			vertices.Add(new Vector3(width, -height, -depth));  // 5
			vertices.Add(new Vector3(width, height, -depth));   // 6

			vertices.Add(new Vector3(width, -height, depth));  // 1
			vertices.Add(new Vector3(width, height, -depth));  // 6
			vertices.Add(new Vector3(width, height, depth));   // 2
			
			// Create vertices for the Left face
			vertices.Add(new Vector3(-width, -height, depth));  // 0
			vertices.Add(new Vector3(-width, -height, -depth)); // 4
			vertices.Add(new Vector3(-width, height, -depth));  // 7

			vertices.Add(new Vector3(-width, -height, depth));  // 0
			vertices.Add(new Vector3(-width, height, -depth));  // 7
			vertices.Add(new Vector3(-width, height, depth));   // 3
		}

		/// <summary>
		/// Creates the normals for the rectangle vertices.
		/// </summary>
		/// <param name="normals">Collection of normal to populate</param>
		private void CreateRectangleNormals(List<Vector3> normals)
		{			
			// Create vertices for the Front face
			normals.Add(new Vector3(0.0f, 0.0f, 1.0f));  // 0
			normals.Add(new Vector3(0.0f, 0.0f, 1.0f));  // 0
			normals.Add(new Vector3(0.0f, 0.0f, 1.0f));  // 0
			normals.Add(new Vector3(0.0f, 0.0f, 1.0f));  // 0
			normals.Add(new Vector3(0.0f, 0.0f, 1.0f));  // 0
			normals.Add(new Vector3(0.0f, 0.0f, 1.0f));  // 0

			// Create vertices for the Back face			
			normals.Add(new Vector3(0.0f, 0.0f, -1.0f)); // 1
			normals.Add(new Vector3(0.0f, 0.0f, -1.0f)); // 1
			normals.Add(new Vector3(0.0f, 0.0f, -1.0f)); // 1
			normals.Add(new Vector3(0.0f, 0.0f, -1.0f)); // 1
			normals.Add(new Vector3(0.0f, 0.0f, -1.0f)); // 1
			normals.Add(new Vector3(0.0f, 0.0f, -1.0f)); // 1

			// Create vertices for the Top face
			normals.Add(new Vector3(0.0f, 1.0f, 0.0f));  // 2
			normals.Add(new Vector3(0.0f, 1.0f, 0.0f));  // 2
			normals.Add(new Vector3(0.0f, 1.0f, 0.0f));  // 2
			normals.Add(new Vector3(0.0f, 1.0f, 0.0f));  // 2
			normals.Add(new Vector3(0.0f, 1.0f, 0.0f));  // 2
			normals.Add(new Vector3(0.0f, 1.0f, 0.0f));  // 2

			// Create vertices for the Bottom face
			normals.Add(new Vector3(0.0f, -1.0f, 0.0f)); // 3
			normals.Add(new Vector3(0.0f, -1.0f, 0.0f)); // 3
			normals.Add(new Vector3(0.0f, -1.0f, 0.0f)); // 3
			normals.Add(new Vector3(0.0f, -1.0f, 0.0f)); // 3
			normals.Add(new Vector3(0.0f, -1.0f, 0.0f)); // 3
			normals.Add(new Vector3(0.0f, -1.0f, 0.0f)); // 3 

			// Create vertices for the Right face
			normals.Add(new Vector3(1.0f, 0.0f, 0.0f));  // 4
			normals.Add(new Vector3(1.0f, 0.0f, 0.0f));  // 4
			normals.Add(new Vector3(1.0f, 0.0f, 0.0f));  // 4
			normals.Add(new Vector3(1.0f, 0.0f, 0.0f));  // 4
			normals.Add(new Vector3(1.0f, 0.0f, 0.0f));  // 4
			normals.Add(new Vector3(1.0f, 0.0f, 0.0f));  // 4

			// Create vertices for the Left face
			normals.Add(new Vector3(-1.0f, 0.0f, 0.0f));  // 5
			normals.Add(new Vector3(-1.0f, 0.0f, 0.0f));  // 5
			normals.Add(new Vector3(-1.0f, 0.0f, 0.0f));  // 5
			normals.Add(new Vector3(-1.0f, 0.0f, 0.0f));  // 5
			normals.Add(new Vector3(-1.0f, 0.0f, 0.0f));  // 5
			normals.Add(new Vector3(-1.0f, 0.0f, 0.0f));  // 5			
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
			              Matrix4.CreateTranslation(Position);
		}

		#endregion
	}
}
