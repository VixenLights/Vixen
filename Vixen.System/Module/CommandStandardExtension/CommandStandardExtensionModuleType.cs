using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;
using CommandStandard;

namespace Vixen.Module.CommandStandardExtension {
	class CommandStandardExtensionModuleType : IModuleLoadNotification {
		public void ModuleLoaded(IModuleDescriptor descriptor) {
			Vixen.Sys.Standard.AddCustomCommand(Modules.GetById(descriptor.TypeId) as ICommandStandardExtension);
		}

		public void ModuleUnloading(IModuleDescriptor descriptor) { }
	}
}
