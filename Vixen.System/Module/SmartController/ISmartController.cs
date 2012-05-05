using Vixen.Sys;
using Vixen.Sys.Output;

namespace Vixen.Module.SmartController {
	public interface ISmartController : IOutputModule, IHardwareModule {
		int OutputCount { get; set; }
		void UpdateState(IntentCollection[] outputStates);
		SmartControllerUpdate UpdateMode { get; }
	}
}
