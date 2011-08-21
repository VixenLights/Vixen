using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;

namespace Vixen.Module.App {
	class AppModuleRepository : IModuleRepository<IAppModuleInstance> {
		private static Dictionary<Guid, IAppModuleInstance> _instances = new Dictionary<Guid, IAppModuleInstance>();

		public void Add(Guid id) {
			// Create a singleton instance.
			IAppModuleInstance instance = Modules.GetById(id) as IAppModuleInstance;
			// Add it to the repository.
			_instances[id] = instance;
			// Assign the AOM reference for the client application.
			instance.Application = ApplicationServices.ClientApplication;
			// Get the module's data from the user data.
			VixenSystem.ModuleData.GetModuleTypeData(instance);
			// Call Loading.
			instance.Loading();
		}

		public IAppModuleInstance Get(Guid id) {
			IAppModuleInstance instance = null;
			_instances.TryGetValue(id, out instance);
			return instance;
		}

		public IAppModuleInstance[] GetAll() {
			return _instances.Values.ToArray();
		}

		public void Remove(Guid id) {
			IAppModuleInstance instance = Get(id);
			if(_instances.Remove(id)) {
				instance.Unloading();
			}
		}

		object IModuleRepository.Get(Guid id) {
			return Get(id);
		}

		object[] IModuleRepository.GetAll() {
			return GetAll();
		}
	}
}
