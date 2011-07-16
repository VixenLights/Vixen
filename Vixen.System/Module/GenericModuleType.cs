using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.Module {
	class GenericModuleType : IModuleLoadNotification {
		public void ModuleLoaded(IModuleDescriptor descriptor) { }
		public void ModuleUnloading(IModuleDescriptor descriptor) { }
	}
}
