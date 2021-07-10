using OpenTK;
using System.Collections.Generic;
using Vector3 = OpenTK.Vector3;


namespace VixenModules.Preview.VixenPreview.Fixtures.OpenGL.Volumes
{
	/// <summary>
	/// Base class of an OpenGL graphical volume.
	/// </summary>
	public abstract class VolumeBase : IVolume
	{
		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="isDynamic">True if volume dynamically changes during execution</param>
		public VolumeBase(bool isDynamic)
		{
			// Default all the matrices to the identity matrix
			ModelMatrix = Matrix4.Identity;			
			ModelMatrix = Matrix4.Identity;			

			// Default the position to zero
			Position = Vector3.Zero;

			// Default the rotation to none
			Rotation = Vector3.Zero;

			// Initialize the group translation to zero
			GroupTranslation = Vector3.Zero;

			// Initialize the group rotation to zero
			GroupRotation = Vector3.Zero;

			// Store off whether the volume is dynamic
			IsDynamic = isDynamic;

			// Create the collection of vertices that make up the volume
			Vertices = new List<Vector3>();

			// Create the collection of normal vectors
			Normals = new List<Vector3>();
									
			// Default to the volume being 			
			Visible = true;
		}

		#endregion
		
		#region IVolume

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>
		public Vector3 Position { get; set; }

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>
		public Vector3 Rotation { get; set; }

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>
		public Matrix4 ModelMatrix { get; set; }
				
		/// <summary>
		/// Refer to interface documentation.
		/// </summary>
		public bool Visible
		{
			get;
			set;
		}

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>
		public bool IsDynamic { get; private set; }
		
		/// <summary>
		/// Refer to interface documentation.
		/// </summary>
		public virtual bool VertexDataIsDirty
		{
			get; set;
		}

		/// <summary>
		/// Refer to interface documentaion.
		/// </summary>		
		public virtual Vector3[] GetNormals()
		{
			return Normals.ToArray();
		}

		/// <summary>
		/// Refer to interface documentaion.
		/// </summary>		
		public virtual Vector3[] GetVertices()
		{
			return Vertices.ToArray();
		}

		/// <summary>
		/// Refer to interface documentaion.
		/// </summary>		
		public abstract void UpdateModelMatrix();

		/// <summary>
		/// Refer to interface documentaion.
		/// </summary>
		public Vector3 GroupTranslation { get; set; }

		/// <summary>
		/// Refer to interface documentaion.
		/// </summary>
		public Vector3 GroupRotation { get; set; }
		
		#endregion

		#region Protected Properties

		/// <summary>
		/// Collection of vertices that make up the volume.
		/// </summary>
		protected List<Vector3> Vertices { get; private set; }

		/// <summary>
		/// Collection of vertex normals.
		/// </summary>
		protected List<Vector3> Normals { get; private set; }

		#endregion		
	}
}
