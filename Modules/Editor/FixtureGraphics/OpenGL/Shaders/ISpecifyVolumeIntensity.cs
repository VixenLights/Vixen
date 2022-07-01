namespace VixenModules.Editor.FixtureGraphics.OpenGL.Shaders
{
	/// <summary>
	/// Maintains the intensity of a volume.
	/// </summary>
	public interface ISpecifyVolumeIntensity
	{
		/// <summary>
		/// Intensity of the volume.
		/// </summary>
		int Intensity { get; set; }
	}
}
