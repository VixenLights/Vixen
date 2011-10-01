using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;

namespace Vixen.Module.CommandStandardExtension {
	class CommandStandardExtensionModuleManagement : GenericModuleManagement<ICommandStandardExtensionModuleInstance> {
		public ICommandStandardExtensionModuleInstance Get(byte platformValue, string extensionName) {
			CommandStandardExtensionModuleRepository repository = Modules.GetRepository<ICommandStandardExtensionModuleInstance, CommandStandardExtensionModuleRepository>();
			return repository.Get(platformValue, extensionName);
		}
	}
}
