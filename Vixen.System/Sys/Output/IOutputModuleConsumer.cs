using Vixen.Module;

namespace Vixen.Sys.Output {
	interface IOutputModuleConsumer : IModuleConsumer, IHardware, IHasSetup {
		int UpdateInterval { get; }
		IOutputDeviceUpdateSignaler UpdateSignaler { get; }
	}
}
