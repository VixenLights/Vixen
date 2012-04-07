using Vixen.Commands;
using Vixen.Sys.Output;

// Controllers implement the idea of outputs because they model a hardware controller that has hardware outputs.
// Previews have no such real-life model and therefore don't implement outputs or chaining.

namespace Vixen.Module.Controller {
    public interface IController : IOutputModule {
		void Start(int outputCount);
		// As defined by the owning controller.
        int OutputCount { get; set; }
		//// One command for each output as defined by the owning controller.
		void UpdateState(ICommand[] outputStates);
		//int UpdateInterval { get; }
    	int ChainIndex { get; set; }
		//IDataPolicy DataPolicy { get; }
	}
}
