using Vixen.Sys;
using Vixen.Sys.Output;

namespace Vixen.Module.SmartController {
	public interface ISmartController : IOutputModule, IHardwareModule, IHasOutputs {
		//int OutputCount { get; set; }
		void UpdateState(IntentChangeCollection[] outputStates);
	}
}
