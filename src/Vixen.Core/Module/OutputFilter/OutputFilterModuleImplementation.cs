using Vixen.Sys.Attribute;

namespace Vixen.Module.OutputFilter
{
	[TypeOfModule("OutputFilter")]
	internal class OutputFilterModuleImplementation : ModuleImplementation<IOutputFilterModuleInstance>
	{
		public OutputFilterModuleImplementation()
			: base(new OutputFilterModuleManagement(), new OutputFilterModuleRepository())
		{
		}
	}
}