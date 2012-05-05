using Vixen.Commands;
using Vixen.Sys;
using Vixen.Sys.Output;

namespace Vixen.Module.Controller {
	public interface IController : IOutputModule, IHardwareModule, IHasOutputs {
		//int OutputCount { get; set; }
		void UpdateState(ICommand[] outputStates);
    	int ChainIndex { get; set; }
		IDataPolicy DataPolicy { get; }
	}
}
