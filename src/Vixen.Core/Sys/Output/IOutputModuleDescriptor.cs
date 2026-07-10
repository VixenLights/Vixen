using Vixen.Module;

namespace Vixen.Sys.Output
{
	public interface IOutputModuleDescriptor : IModuleDescriptor
	{
		int UpdateInterval { get; }
	}
}