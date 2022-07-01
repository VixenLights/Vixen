using System.Drawing;

namespace VixenModules.Editor.FixtureGraphics.OpenGL.Volumes
{
	/// <summary>
	/// Maintains the color of a graphical volume.
	/// </summary>
	public interface ISpecifyVolumeColor
	{
		/// <summary>
		/// Gets or sets the color of the volume.
		/// </summary>
		Color Color
		{
			get;
			set;
		}
	}
}
