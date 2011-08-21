using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;

namespace Vixen.Module.Script {
	[TypeOfModule("Script")]
	class ScriptModuleImplementation : ModuleImplementation<IScriptModuleInstance> {
		public ScriptModuleImplementation()
			: base(new ScriptModuleManagement(), new ScriptModuleRepository()) {
		}
	}
}
