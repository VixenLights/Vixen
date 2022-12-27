using Vixen.Sys.Attribute;

namespace Vixen.Module.Input
{
	[TypeOfModule("Input")]
	internal class InputModuleImplementation : ModuleImplementation<IInputModuleInstance>
	{
		public InputModuleImplementation()
			: base(new InputModuleManagement(), new InputModuleRepository())
		{
		}
	}
}