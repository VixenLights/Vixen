using Vixen.Module;
using Vixen.Execution;

namespace Vixen.Sys {
	public interface IHardwareModule : IModuleInstance, IExecutionControl, IHasSetup {
		bool IsRunning { get; }
		bool IsPaused { get; }
	}
}
