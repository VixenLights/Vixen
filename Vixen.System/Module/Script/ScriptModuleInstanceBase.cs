using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;

namespace Vixen.Module.Script {
	abstract public class ScriptModuleInstanceBase : ModuleInstanceBase, IScriptModuleInstance, IEqualityComparer<IScriptModuleInstance> {
		public bool Equals(IScriptModuleInstance x, IScriptModuleInstance y) {
			return base.Equals(x, y);
		}

		public int GetHashCode(IScriptModuleInstance obj) {
			return base.GetHashCode(obj);
		}
	}
}
