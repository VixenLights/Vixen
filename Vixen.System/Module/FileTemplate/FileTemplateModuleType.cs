using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;

namespace Vixen.Module.FileTemplate {
	class FileTemplateModuleType : IModuleLoadNotification {
		public void ModuleLoaded(IModuleDescriptor descriptor) {
			Modules.ModuleRepository.AddFileTemplate(descriptor.TypeId);
		}

		public void ModuleUnloading(IModuleDescriptor descriptor) {
			Modules.ModuleRepository.RemoveFileTemplate(descriptor.TypeId);
		}
	}
}
