using Vixen.Module;
using Vixen.Execution;

namespace Vixen.Sys {
	public interface IHardwareModule : IModuleInstance, IExecutionControl, ISetup {
		bool IsRunning { get; }
    }
}
