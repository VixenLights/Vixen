using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Common;

namespace Vixen.Module.CommandStandardExtension {
	[ModuleType("CommandStandardExtension")]
	class CommandStandardExtensionModuleImplementation : ModuleImplementation<ICommandStandardExtension> {
		public CommandStandardExtensionModuleImplementation()
			: base(new CommandStandardExtensionModuleType(), new CommandStandardExtensionModuleManagement(), new CommandStandardExtensionModuleRepository()) {
		}
	}
}
