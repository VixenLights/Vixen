using Vixen.Sys;

namespace Vixen.Module.Intent {
	[TypeOfModule("Intent")]
	class IntentModuleImplementation : ModuleImplementation<IIntentModuleInstance> {
		public IntentModuleImplementation()
			: base(new IntentModuleManagement(), new IntentModuleRepository()) {
		}
	}
}
