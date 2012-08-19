using Vixen.Commands;
using Vixen.Sys;

namespace Vixen.Module.Controller {
	/// <summary>
	/// Core abstraction for the controller module.
	/// </summary>
	public interface IController {
		void UpdateState(int chainIndex, ICommand[] outputStates);
		IDataPolicy DataPolicy { get; }
	}
}
