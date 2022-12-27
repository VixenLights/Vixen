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
