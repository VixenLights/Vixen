using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Common;
using Vixen.Sys;

namespace Vixen.Module.CommandSpec {
	class CommandSpecModuleType : IModuleLoadNotification {
		public void ModuleLoaded(IModuleDescriptor descriptor) {
			Server.ModuleRepository.AddCommandSpec(descriptor.TypeId);
		}

		public void ModuleUnloading(IModuleDescriptor descriptor) {
			Server.ModuleRepository.RemoveCommandSpec(descriptor.TypeId);
		}
	}
}
