using Vixen.Sys.Attribute;

namespace Vixen.Module.Trigger
{
	[TypeOfModule("Trigger")]
	internal class TriggerModuleImplementation : ModuleImplementation<ITriggerModuleInstance>
	{
		public TriggerModuleImplementation()
			: base(new TriggerModuleManagement(), new TriggerModuleRepository())
		{
		}
	}
}