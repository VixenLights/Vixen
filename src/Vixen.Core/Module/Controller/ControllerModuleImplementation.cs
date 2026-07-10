using Vixen.Sys.Attribute;

namespace Vixen.Module.Controller
{
	[TypeOfModule("Controller")]
	internal class ControllerModuleImplementation : ModuleImplementation<IControllerModuleInstance>
	{
		public ControllerModuleImplementation()
			: base(new ControllerModuleManagement(), new ControllerModuleRepository())
		{
		}
	}
}