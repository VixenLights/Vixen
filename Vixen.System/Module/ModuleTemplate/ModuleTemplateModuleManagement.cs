using System.Collections.Generic;
using System.Linq;
using Vixen.Sys;

namespace Vixen.Module.ModuleTemplate {
	class ModuleTemplateModuleManagement : GenericModuleManagement<IModuleTemplateModuleInstance> {
		public void ProjectTemplateInto(IModuleInstance target) {
			// Get all template module descriptors.
			IEnumerable<IModuleTemplateModuleDescriptor> templateDescriptors = Modules.GetDescriptors<IModuleTemplateModuleInstance, IModuleTemplateModuleDescriptor>();
			// Find the one for the module type.
			// (i.e. Has the module type as a dependency.)
			IModuleTemplateModuleDescriptor descriptor = templateDescriptors.FirstOrDefault(x => x.Dependencies.Contains(target.Descriptor.TypeId));
			if(descriptor != null) {
				// Get an instance of the template module.
				IModuleTemplateModuleInstance instance = Get(descriptor.TypeId);
				// Project the template into the target instance.
				instance.Project(target);
			}
		}
	}
}
