using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;

namespace Vixen.Module.App {
	class AppModuleManagement : IModuleManagement<IAppModuleInstance> {
		public IAppModuleInstance Get(Guid id) {
			return Modules.ModuleRepository.GetApp(id);
		}

		object IModuleManagement.Get(Guid id) {
			return Get(id);
		}

		public IAppModuleInstance[] GetAll() {
			return Modules.ModuleRepository.GetAllApp();
		}

		object[] IModuleManagement.GetAll() {
			return GetAll();
		}

		public IAppModuleInstance Clone(IAppModuleInstance instance) {
			// These are singletons.
			return null;
		}

		object IModuleManagement.Clone(object instance) {
			return Clone(instance as IAppModuleInstance);
		}
	}
}
