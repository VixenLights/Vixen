using Vixen.Sys.Attribute;

namespace Vixen.Module.OutputFilter {
	[TypeOfModule("OutputFilter")]
	class OutputFilterModuleImplementation : ModuleImplementation<IOutputFilterModuleInstance> {
		public OutputFilterModuleImplementation()
			: base(new OutputFilterModuleManagement(), new OutputFilterModuleRepository()) {
		}
	}
}
