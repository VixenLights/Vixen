using System.Collections.Generic;
using System.Numerics;
using VixenModules.Preview.VixenPreview.Fixtures.OpenGL.Volumes;

namespace VixenModules.Preview.VixenPreview.Fixtures.OpenGL.Shaders
{
	/// <summary>
	/// Maintains a volume shader program.
	/// </summary>
	public interface IVolumeShader
	{
		/// <summary>
		/// Activates the shader program for drawing.
		/// </summary>
		void Use();

		/// <summary>
		/// Transfer the uniforms for the specified graphical volumes.
		/// </summary>
		/// <param name="volumes">Volumes to transfer uniform data and draw</param>
		/// <param name="lightPosition">Position of the light source</param>
		void TransferUniformsAndDraw(IEnumerable<IVolume> volumes, Vector3 lightPosition);

		/// <summary>
		/// Transfers the vertex and normal data associated with the specified graphical volumes to the GPU.
		/// </summary>
		/// <param name="volumes">Volumes to transfer to the GPU</param>
		void TransferBuffers(IEnumerable<IVolume> volumes);
	}
}
