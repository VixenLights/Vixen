using Vixen.Sys;
using Vixen.Sys.Output;

namespace Vixen.Module.SmartController
{
	/// <summary>
	/// Core abstraction for the smart controller module.
	/// </summary>
	public interface ISmartController : IUpdatableOutputCount
	{
		void UpdateState(IntentChangeCollection[] outputStates);
	}
}