using Vixen.Sys;

namespace Vixen.Module.SmartController {
	/// <summary>
	/// Core abstraction for the smart controller module.
	/// </summary>
	public interface ISmartController {
		void UpdateState(IntentChangeCollection[] outputStates);
	}
}
