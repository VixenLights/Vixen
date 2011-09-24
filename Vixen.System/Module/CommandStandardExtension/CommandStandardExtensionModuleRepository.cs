using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;

namespace Vixen.Module.CommandStandardExtension {
	class CommandStandardExtensionModuleRepository : GenericModuleRepository<ICommandStandardExtensionModuleInstance> {
		public override void Add(Guid id) {
			VixenStandard.AddCustomCommand(Modules.ModuleManagement.GetCommandStandardExtension(id));
		}
	}
}
