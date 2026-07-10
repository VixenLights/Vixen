using Vixen.Sys.Attribute;

namespace Vixen.Module.SmartController
{
	[TypeOfModule("SmartController")]
	internal class SmartControllerModuleImplementation : ModuleImplementation<ISmartControllerModuleInstance>
	{
		public SmartControllerModuleImplementation()
			: base(new SmartControllerModuleManagement(), new SmartControllerModuleRepository())
		{
		}
	}
}