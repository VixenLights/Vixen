using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Common;

namespace Vixen.Module.CommandSpec {
	[ModuleType("CommandSpec")]
	class CommandSpecModuleImplementation : ModuleImplementation<ICommandSpecModuleInstance> {
		public CommandSpecModuleImplementation()
			: base(new CommandSpecModuleType(), new CommandSpecModuleManagement(), new CommandSpecModuleRepository()) {
		}
	}
}
