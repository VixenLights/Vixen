using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;
using Vixen.Module.CommandStandardExtension;

namespace Vixen.Commands {
	class CustomCommandBehavior : ICustomCommandBehavior {
		public const byte Value = byte.MaxValue;

		private Dictionary<string, ICommandStandardExtensionModuleInstance> _loadedExtensions = new Dictionary<string, ICommandStandardExtensionModuleInstance>();

		public Command Get(byte platformValue, string extensionName) {
			ICommandStandardExtensionModuleInstance module;

			if(!_loadedExtensions.TryGetValue(extensionName, out module)) {
				// Get the manager.
				CommandStandardExtensionModuleManagement manager = Modules.GetManager<ICommandStandardExtensionModuleInstance, CommandStandardExtensionModuleManagement>();
				// Get the module instance.
				module = manager.Get(platformValue, extensionName);
				if(module != null) {
					// Cache.
					_loadedExtensions[extensionName] = module;
				} else {
					return null;
				}
			}

			// Get the command instance.
			return module.GetCommand();
		}
	}
}
