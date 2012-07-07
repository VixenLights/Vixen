using System;
using System.Collections.Generic;
using System.Linq;
using Vixen.Services;
using Vixen.Sys;

namespace Vixen.Module.App {
	class AppModuleRepository : IModuleRepository<IAppModuleInstance> {
		//private static Dictionary<Guid, IAppModuleInstance> _instances = new Dictionary<Guid, IAppModuleInstance>();
		private SingletonRepository<IAppModuleInstance> _repository;

		public AppModuleRepository() {
			_repository = new SingletonRepository<IAppModuleInstance>();
		}

		public void Add(Guid id) {
			// Create a singleton instance.
			IAppModuleInstance instance = (IAppModuleInstance)Modules.GetById(id);
			// Add it to the repository.
			//_instances[id] = instance;
			_repository.Add(instance);
			// Assign the AOM reference for the client application.
			instance.Application = ApplicationServices.ClientApplication;
			// Call Loading.
			instance.Loading();
		}

		public IAppModuleInstance Get(Guid id) {
			return _repository.Get(id);
			//IAppModuleInstance instance;
			//_instances.TryGetValue(id, out instance);
			//return instance;
		}

		public IAppModuleInstance[] GetAll() {
			return _repository.GetAll();
			//return _instances.Values.ToArray();
		}

		public void Remove(Guid id) {
			IAppModuleInstance instance = Get(id);
			//if(_instances.Remove(id)) {
			//    instance.Unloading();
			//}
			if(_repository.Remove(id)) {
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
