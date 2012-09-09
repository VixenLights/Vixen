using Vixen.Module;

namespace Vixen.Sys.Output {
	interface IOutputModuleConsumer<out T> : IModuleConsumer<T>, IHardware, IHasSetup
		where T : class, IOutputModule {
		int UpdateInterval { get; }
		IOutputDeviceUpdateSignaler UpdateSignaler { get; }
	}
}
