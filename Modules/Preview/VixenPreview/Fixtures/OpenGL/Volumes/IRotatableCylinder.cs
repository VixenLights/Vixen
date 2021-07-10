using OpenTK;

namespace VixenModules.Preview.VixenPreview.Fixtures.OpenGL.Volumes
{
	/// <summary>
	/// Defines properties that allow a cylinder to rotate around a specified axis.
	/// </summary>
	public interface IRotatableCylinder : IVolume
	{
		/// <summary>
		/// Tilt rotation of the cylinder.
		/// </summary>
		Vector3 TiltRotation { get; set; }

		/// <summary>
		/// Translation that define the point of rotation.
		/// </summary>
		Vector3 TiltTranslation { get; set; }
	}
}
