using Vixen.Sys.Attribute;

namespace Vixen.Module.SequenceFilter {
	[TypeOfModule("SequenceFilter")]
	class SequenceFilterModuleImplementation : ModuleImplementation<ISequenceFilterModuleInstance> {
		public SequenceFilterModuleImplementation()
			: base(new SequenceFilterModuleManagement(), new SequenceFilterModuleRepository()) {
		}
	}
}
