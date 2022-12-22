namespace Vixen.Module.MediaRenderer
{
	public interface IMediaRendererModuleDescriptor : IModuleDescriptor
	{
		string[] FileExtensions { get; }
	}
}