using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;

namespace Vixen.Module.App {
	class AppModuleType : IModuleLoadNotification {
		public void ModuleLoaded(IModuleDescriptor descriptor) {
			// Add it to the repository.
			Modules.ModuleRepository.AddApp(descriptor.TypeId);
		}

		public void ModuleUnloading(IModuleDescriptor descriptor) {
			Modules.ModuleRepository.RemoveApp(descriptor.TypeId);
		}
	}
}
