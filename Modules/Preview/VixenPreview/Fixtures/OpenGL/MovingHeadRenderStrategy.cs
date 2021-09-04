using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using VixenModules.Preview.VixenPreview.Fixtures.OpenGL.Shaders;
using VixenModules.Preview.VixenPreview.Fixtures.OpenGL.Volumes;

namespace VixenModules.Preview.VixenPreview.Fixtures.OpenGL
{
	/// <summary>
	/// This class helps efficiently render multiple fixtures.  This class is grouping the volumes that make up these
	/// fixtures by shader.  Additionally there is concept of static volumes and dynamic volumes.  The vertices that
	/// make up a static volume only need to be sent to the GPU once.  Where dynamic volumes may need to send their
	/// vertices more often.
	/// </summary>
	public class MovingHeadRenderStrategy : IDisposable
	{
		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		public MovingHeadRenderStrategy()
		{			
			// Create the shaders for the DMX fixtures
			_grayShader = new GrayVolumeShader();			
			_beamShader = new ColorVolumeShader();

			// Create the collection of shapes to render
			Shapes = new List<IRenderMovingHeadOpenGL>();
		}

		#endregion

		#region Fields

		/// <summary>
		/// Gray shader for the fixture housing.
		/// </summary>
		private VolumeShader _grayShader;
		
		/// <summary>
		/// Colored shader for the fixture beam.
		/// </summary>
		private VolumeShader _beamShader;
		
		/// <summary>
		/// Grouping of static volumes by shader.
		/// </summary>
		private IEnumerable<IGrouping<Guid, Tuple<IVolume, Guid>>> _staticVolumesGroupedByShader;
		
		/// <summary>
		/// Grouping of dynamic volumes by shader.
		/// </summary>
		private IEnumerable<IGrouping<Guid, Tuple<IVolume, Guid>>> _dynamicVolumesGroupedByShader;

		/// <summary>
		/// Flag that indicates whether the static vertices have been transferred to the GPU.
		/// </summary>
		private bool _staticDataTransferred = false;

		/// <summary>
		/// Flag that indicates whether the dynamic vertices have been transferred to the GPU.
		/// </summary>
		private bool _dynamicDataTransferred = false;

		/// <summary>
		/// Projection matrix previously sent to the GPU.  This field allows the class to
		/// only send the projection matrix to the GPU when it changes.
		/// </summary>
		private Matrix4 _previousProjectionMatrix = Matrix4.Identity;

		/// <summary>
		/// View matrix previously sent to the GPU.  This field allows the class to
		/// only send the view matrix to the GPU when it changes.
		/// </summary>
		private Matrix4 _previousViewMatrix = Matrix4.Identity;

		/// <summary>
		/// Camera position previously sent to the GPU.  This field allows the class to
		/// only send the camera position to the GPU when it changes.
		/// </summary>
		private Vector3 _previousCamera = Vector3.Zero;
		
		#endregion

		#region Public Properties

		/// <summary>
		/// Collection of shapes to render.
		/// </summary>
		public List<IRenderMovingHeadOpenGL> Shapes { get; private set; }

		#endregion

		#region Public Methods

		/// <summary>
		/// Initializes the render strategy.  This method should be called after configuring the strategy with the shapes to render.
		/// </summary>
		public void Initialize()
		{
			// Create a collection to hold all volumes from all the shapes
			List<Tuple<IVolume, Guid>> volumes = new List<Tuple<IVolume, Guid>>();

			// Loop over all the fixture shapes 
			foreach (var shape in Shapes)
			{
				// Retrieve the volumes from the shape and add them to the overall collection
				volumes.AddRange(shape.GetVolumes());
			}
			
			// Retrieve the static volumes
			List<Tuple<IVolume, Guid>> staticVolumes = volumes.Where(vol => !vol.Item1.IsDynamic).ToList();

			// Group the static volumes by shader ID
			_staticVolumesGroupedByShader = staticVolumes.GroupBy(volumeTuple => volumeTuple.Item2);

			// Retrieve the dynamic volumes
			List<Tuple<IVolume, Guid>> dynamicVolumes = volumes.Where(vol => vol.Item1.IsDynamic).ToList();

			// Group the dynamic volumes by shader ID
			_dynamicVolumesGroupedByShader = dynamicVolumes.GroupBy(volumeTuple => volumeTuple.Item2);			
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Retrieves the shader associated with the specified GUID.
		/// </summary>
		/// <param name="shaderID">Unique identifier for the shader</param>
		/// <returns>Shader associated with the GUID</returns>
		private VolumeShader RetrieveShader(Guid shaderID)
		{
			// Default to the gray shader
			VolumeShader shader = _grayShader;

			// Check to see if the colored shader was requested
			if (shaderID == ColorVolumeShader.ShaderID)			
			{
				// Return the colored shader
				shader = _beamShader;
			}

			return shader;
		}

		/// <summary>
		/// Updates the projection and model matrices on the volumes.
		/// </summary>		
		/// <param name="volumes">Volumes to update</param>
		private void PrepareVolumesForRendering(IEnumerable<IVolume> volumes)
		{
			// Loop over the volumes
			foreach (IVolume v in volumes)
			{
				// Update the model matrix
				v.UpdateModelMatrix();				
			}
		}

		/// <summary>
		/// Renders the specified volumes with the specified shader.
		/// </summary>
		/// <param name="volumes">Volumes to render</param>
		/// <param name="shader">Shader to use to render the volumes</param>
		/// <param name="projectionMatrix">Project matrix to use during rendering</param>
		/// <param name="camera">Camera to use during rendering</param>		
		/// <param name="vertexDataTransferredOnce">Flag used to keep track if the vertex data has been tranferred at least once</param>
		/// <param name="vertexDataIsDirty">Whether the vertex data is dirty (stale)</param>
		private void RenderVolumes(
			List<IVolume> volumes, 
			VolumeShader shader, 
			Matrix4? projectionMatrix,
			Matrix4? viewMatrix,
			Vector3? camera,
			ref bool vertexDataTransferredOnce,
			bool vertexDataIsDirty)
		{			
			// Prepare the volumes for rendering by updating projection and model matrices
			PrepareVolumesForRendering(volumes);

			// Activate the shader
			shader.Use();

			// Bind the vertex array
			GL.BindVertexArray(shader.VaoID);

			// If the vertex data is dirty or the vertex data has not been transferred at least once then...
			if (vertexDataIsDirty || !vertexDataTransferredOnce)
			{
				// Transfer the volume vertex data
				shader.TransferBuffers(volumes);

				// Remember that we have transferred the vertex data
				vertexDataTransferredOnce = true;

				// Loop over the volumes
				foreach(IVolume volume in volumes)
				{
					// Clear the vertext dirty flag
					volume.VertexDataIsDirty = false;
				}
			}

			// Transfer the uniforms and draw the volumes
			shader.TransferUniformsAndDraw(volumes, camera, projectionMatrix, viewMatrix);

			// Clear the vertex array
			GL.BindVertexArray(0);

			// Clear the shader 
			GL.UseProgram(0);
		}

		/// <summary>
		/// Renders the static volumes associated with the shapes.
		/// </summary>
		/// <param name="projectionMatrix">Projection matrix to use during rendering</param>
		/// <param name="camera">Camera position to use during rendering</param>		
		/// <param name="viewMatrix">View matrix to use during rendering</param>
		/// <param name="backgroundAlpha">Alpaha component of the preview background</param>
		private void RenderStaticVolumes(Matrix4? projectionMatrix, Vector3? camera, Matrix4? viewMatrix, int backgroundAlpha)
		{
			// Loop over the static volume groups organized by shader type
			foreach (IGrouping<Guid, Tuple<IVolume, Guid>> shaderGroup in _staticVolumesGroupedByShader)
			{
				// Retrieve the shader associated with the GUID
				VolumeShader shader = RetrieveShader(shaderGroup.Key);

				// If the shader supports intensity then...
				if (shader is ISpecifyVolumeIntensity)
				{
					// Give the shader the background alpha as the intensity
					((ISpecifyVolumeIntensity)shader).Intensity = backgroundAlpha;
				}

				// Extract the static volumes
				List<IVolume> staticVolumes = shaderGroup.Select(vol => vol.Item1).ToList();

				// Render the static volumes
				RenderVolumes(staticVolumes, shader, projectionMatrix, viewMatrix, camera, ref _staticDataTransferred, false);				
			}
		}

		/// <summary>
		/// Renders the dynamic volumes associated with the shapes.
		/// </summary>
		/// <param name="projectionMatrix">Project matrix to use during rendering</param>
		/// <param name="camera">Camera position to use during rendering</param>		
		/// <param name="viewMatrix">View matrix to use during rendering</param>
		private void RenderDynamicVolumes(Matrix4? projectionMatrix, Vector3? camera, Matrix4? viewMatrix)
		{
			// Loop over the dynamic volume groups organized by shader type
			foreach (IGrouping<Guid, Tuple<IVolume, Guid>> shaderGroup in _dynamicVolumesGroupedByShader)
			{				
				// Retrieve the shader associated with the GUID
				VolumeShader shader = RetrieveShader(shaderGroup.Key);

				// Extract the dynamic volumes
				List<IVolume> dynamicVolumes = shaderGroup.Select(vol => vol.Item1).ToList();

				// Determine if any of the vertex data is dirty
				bool dynamicVertexDataIsDirty = dynamicVolumes.Any(volume => volume.VertexDataIsDirty);

				// Render the dynamic volumes
				RenderVolumes(dynamicVolumes, shader, projectionMatrix, viewMatrix, camera, ref _dynamicDataTransferred, dynamicVertexDataIsDirty);				
			}
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Renders the volumes associated with shapes.
		/// </summary>
		/// <param name="projectionMatrix">Projection matrix to use during rendering</param>
		/// <param name="camera">Camera position to use during rendering</param>
		/// <param name="viewMatrix">View matrix to use during rendering</param>
		/// <param name="backgroundAlpha">Alpaha component of the preview background</param>
		public void RenderVolumes(Matrix4 projectionMatrix, Vector3 camera, Matrix4 viewMatrix, int backgroundAlpha)
		{
			// Declare nullable projection matrix
			// When the matrix is null it won't be sent to the GPU
			Matrix4? optimizedProjectionMatrix = null;
			
			// If the projection matrix is different from the last frame then...
			if (_previousProjectionMatrix != projectionMatrix)
			{
				// Update the nullable projection matrix to the one specified by the caller
				optimizedProjectionMatrix = projectionMatrix;
				
				// Store off the new projection matrix
				_previousProjectionMatrix = projectionMatrix;
			}

			// Declare nullable camera position
			// When the camera position is null it won't be sent to the GPU
			Vector3? optimizedCamera = null;

			// If the camera position is different from the last frame then...
			if (_previousCamera != camera)
			{
				// Update the nullable camera position to the one specified by the caller
				optimizedCamera = camera;

				// Store off the new camera position
				_previousCamera = camera;
			}

			// Declare nullable view matrix
			// When the matrix is null it won't be sent to the GPU
			Matrix4? optimizedViewMatrix = null;

			// If the view matrix is different from the last frame then...
			if (_previousViewMatrix != viewMatrix)
			{
				// Update the nullable view matrix to the one specified by the caller
				optimizedViewMatrix = viewMatrix;

				// Store off the new view matrix
				_previousViewMatrix = viewMatrix;
			}

			// Render the static volumes
			RenderStaticVolumes(optimizedProjectionMatrix, optimizedCamera, optimizedViewMatrix, backgroundAlpha);

			// Render the dynamic volumes
			RenderDynamicVolumes(optimizedProjectionMatrix, optimizedCamera, optimizedViewMatrix);									
		}

		#endregion

		#region IDisposable

		/// <summary>
		/// Refer to MSDN documentation.
		/// </summary>
		public void Dispose()
		{
			// If the gray shader was created then...
			if (_grayShader != null)
			{
				// Dispose of the gray shader
				_grayShader.Dispose();
				_grayShader = null;
			}

			// If the colored beam shader was created then...
			if (_beamShader != null)
			{
				// Dispose of the beam shader
				_beamShader.Dispose();
				_beamShader = null;
			}
		}

		#endregion
	}
}
