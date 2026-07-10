using Vixen.Module;

namespace Vixen.Sys.Output
{
	/// <summary>
	/// The module used by an IOutputDevice and the basis for any output module.
	/// </summary>
	public interface IOutputModule : IModuleInstance, IOutputter
	{
	}
}