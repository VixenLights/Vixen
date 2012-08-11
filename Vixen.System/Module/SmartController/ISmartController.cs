using Vixen.Sys;
using Vixen.Sys.Output;

namespace Vixen.Module.SmartController {
	public interface ISmartController : IOutputModule, IHardwareModule, IHasOutputs {
		void UpdateState(IntentChangeCollection[] outputStates);
	}
}
