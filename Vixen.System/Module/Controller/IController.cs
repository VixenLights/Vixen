using Vixen.Commands;
using Vixen.Sys;
using Vixen.Sys.Output;

namespace Vixen.Module.Controller {
	public interface IController : IOutputModule, IHardwareModule, IHasOutputs {
		void UpdateState(ICommand[] outputStates);
    	int ChainIndex { get; set; }
		IDataPolicy DataPolicy { get; }
	}
}
