using Vixen.Sys.Attribute;

namespace Vixen.Module.App {
	[TypeOfModule("App")]
	class AppModuleImplementation : ModuleImplementation<IAppModuleInstance> {
		public AppModuleImplementation()
			: base(new AppModuleManagement(), new AppModuleRepository()) {
		}
	}
}
