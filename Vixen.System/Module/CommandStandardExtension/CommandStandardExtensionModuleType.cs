using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;
using CommandStandard;

namespace Vixen.Module.CommandStandardExtension {
	class CommandStandardExtensionModuleType : IModuleLoadNotification {
		public void ModuleLoaded(IModuleDescriptor descriptor) {
			//Vixen.Sys.Standard.AddCustomCommand(Modules.GetById(descriptor.TypeId) as ICommandStandardExtension);
			Vixen.Sys.Standard.AddCustomCommand(Modules.ModuleManagement.GetCommandStandardExtension(descriptor.TypeId));
		}

		public void ModuleUnloading(IModuleDescriptor descriptor) { }
	}
}
