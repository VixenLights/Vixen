using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;

namespace Vixen.Module.App {
	abstract public class AppModuleInstanceBase : ModuleInstanceBase, IAppModuleInstance, IEqualityComparer<IAppModuleInstance> {
		abstract public void Loading();

		abstract public void Unloading();

		abstract public IApplication Application { set; }

		public bool Equals(IAppModuleInstance x, IAppModuleInstance y) {
			return base.Equals(x, y);
		}

		public int GetHashCode(IAppModuleInstance obj) {
			return base.GetHashCode(obj);
		}
	}
}
