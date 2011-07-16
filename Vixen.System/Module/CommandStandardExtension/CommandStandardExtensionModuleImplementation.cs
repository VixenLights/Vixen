using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Common;

namespace Vixen.Module.CommandStandardExtension {
	[TypeOfModule("CommandStandardExtension")]
	class CommandStandardExtensionModuleImplementation : ModuleImplementation<ICommandStandardExtensionModuleInstance> {
		public CommandStandardExtensionModuleImplementation()
			: base(new CommandStandardExtensionModuleManagement(), new CommandStandardExtensionModuleRepository()) {
		}
	}
}
