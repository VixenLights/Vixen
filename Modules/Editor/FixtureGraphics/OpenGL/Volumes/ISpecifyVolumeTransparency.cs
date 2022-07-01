using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VixenModules.Editor.FixtureGraphics.OpenGL.Volumes
{
	/// <summary>
	/// Maintains the transparency of a volume.
	/// </summary>
	public interface ISpecifyVolumeTransparency
	{
		/// <summary>
		/// Transparency of the volume.
		/// </summary>
		double Transparency { get; set; }
	}
}
