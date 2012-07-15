using Vixen.Module;
using Vixen.Module.ModuleTemplate;
using Vixen.Sys;

namespace Vixen.Services {
	public class ModuleTemplateService {
		static private ModuleTemplateService _instance;

		private ModuleTemplateService() { }

		public static ModuleTemplateService Instance {
			get { return _instance ?? (_instance = new ModuleTemplateService()); }
		}

		public void ProjectTemplateInto(IModuleInstance target) {
			ModuleTemplateModuleManagement manager = Modules.GetManager<IModuleTemplateModuleInstance, ModuleTemplateModuleManagement>();
			manager.ProjectTemplateInto(target);
		}
	}
}
