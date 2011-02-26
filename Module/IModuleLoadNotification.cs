using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.Module {
	interface IModuleLoadNotification {
		void ModuleLoaded(IModuleDescriptor descriptor);
		void ModuleUnloading(IModuleDescriptor descriptor);
	}
}
