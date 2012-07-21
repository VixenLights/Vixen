using System;
using Vixen.Sys;

namespace Vixen.Module.Service {
	class ServiceModuleRepository : IModuleRepository<IServiceModuleInstance> {
		private SingletonRepository<IServiceModuleInstance> _repository;

		public ServiceModuleRepository() {
			_repository = new SingletonRepository<IServiceModuleInstance>();
		}

		public void Add(Guid id) {
			// Create a singleton instance.
			IServiceModuleInstance instance = (IServiceModuleInstance)Modules.GetById(id);
			// Add it to the repository.
			_repository.Add(instance);
			// Start the service.
			instance.Start();
		}

		public IServiceModuleInstance Get(Guid id) {
			return _repository.Get(id);
		}

		public IServiceModuleInstance[] GetAll() {
			return _repository.GetAll();
		}

		public void Remove(Guid id) {
			IServiceModuleInstance instance = Get(id);
			if(_repository.Remove(id)) {
				instance.Stop();
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
