using Vixen.Module.Controller;
using Vixen.Commands;

namespace VixenModules.Output.HelixController
{
	public class HelixModule : ControllerModuleInstanceBase
	{
		public HelixModule()
		{
			DataPolicyFactory = new HelixDataPolicyFactory();
		}

		public override void UpdateState(int chainIndex, ICommand[] outputStates)
		{
	
		}
	}
}
