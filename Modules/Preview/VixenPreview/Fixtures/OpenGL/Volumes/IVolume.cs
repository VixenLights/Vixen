using OpenTK;

namespace VixenModules.Preview.VixenPreview.Fixtures.OpenGL.Volumes
{
	/// <summary>
	/// Maintains a 3-D volume to be rendered.
	/// </summary>
	public interface IVolume
	{
		/// <summary>
		/// True when the volume is to be shown.
		/// </summary>
		bool Visible { get; set; }

		/// <summary>
		/// Gets the normal vectors of the volume for light processing.
		/// </summary>
		/// <returns>Normals for each vertex</returns>
		Vector3[] GetNormals();

		/// <summary>
		/// Gets the vertices of the volume.
		/// </summary>
		/// <returns>Vertices that make up the volume.</returns>
		Vector3[] GetVertices();

		/// <summary>
		/// Update the model matrix of the volume.
		/// </summary>
		void UpdateModelMatrix();

		/// <summary>
		/// Determines if the volume can change geometry during execution.
		/// </summary>		
		bool IsDynamic { get; }

		/// <summary>
		/// Indicates when the vertex data has been modified and needs to be sent to the GPU. 
		/// </summary>		
		bool VertexDataIsDirty
		{
			get; set;
		}

		/// <summary>
		/// Position of the volume.
		/// </summary>
		Vector3 Position { get; set; }

		/// <summary>
		/// Model matrix for the volume.
		/// </summary>
		Matrix4 ModelMatrix { get; set; }
		
		/// <summary>
		/// Group translation of the volume.
		/// </summary>
		Vector3 GroupTranslation { get; set; }

		/// <summary>
		/// Group rotation of the volume.
		/// </summary>
		Vector3 GroupRotation { get; set; }		
	}
}
