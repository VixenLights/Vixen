using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Common;

namespace Vixen.Module.CommandEditor {
	[ModuleType("CommandEditor")]
	class CommandEditorModuleImplementation : ModuleImplementation<ICommandEditorModuleInstance> {
		public CommandEditorModuleImplementation()
			: base(new CommandEditorModuleType(), new CommandEditorModuleManagement(), new CommandEditorModuleRepository()) {
		}
	}
}
