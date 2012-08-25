using System;
using Vixen.Commands;
using Vixen.Sys;

namespace Vixen.Module.Controller {
	/// <summary>
	/// Core abstraction for the controller module.
	/// </summary>
	public interface IController {
		void UpdateState(int chainIndex, ICommand[] outputStates);
		// A factory method instead of just specifying a type because unlike
		// a descriptor or module, this could change at runtime.
		IDataPolicyFactory DataPolicyFactory { get; }
		event EventHandler DataPolicyFactoryChanged;
	}
}
