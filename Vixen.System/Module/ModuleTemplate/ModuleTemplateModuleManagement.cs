using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;
using System.IO;
using System.Xml.Linq;
using Vixen.Common;

namespace Vixen.Module.ModuleTemplate {
	class ModuleTemplateModuleManagement : IModuleManagement<IModuleTemplateModuleInstance> {
		public IModuleTemplateModuleInstance Get(Guid id) {
			// Template modules are singletons.
			// The repository assumes to instantiate the template's data.
			return Modules.ModuleRepository.GetModuleTemplate(id);
		}

		object IModuleManagement.Get(Guid id) {
			return Get(id);
		}

		public IModuleTemplateModuleInstance[] GetAll() {
			return Modules.ModuleRepository.GetAllModuleTemplate();
		}

		object[] IModuleManagement.GetAll() {
			return GetAll();
		}

		public IModuleTemplateModuleInstance Clone(IModuleTemplateModuleInstance instance) {
			// These are singletons.
			return null;
		}

		object IModuleManagement.Clone(object instance) {
			return Clone(instance as IModuleTemplateModuleInstance);
		}

		public void ProjectTemplateInto(IModuleInstance target) {
			// Get all template module descriptors.
			IEnumerable<IModuleTemplateModuleDescriptor> templateDescriptors = Modules.GetModuleDescriptors<IModuleTemplateModuleInstance, IModuleTemplateModuleDescriptor>();
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
