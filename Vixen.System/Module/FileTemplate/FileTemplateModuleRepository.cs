using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;

namespace Vixen.Module.FileTemplate {
	class FileTemplateModuleRepository : IModuleRepository<IFileTemplateModuleInstance> {
		private Dictionary<Guid, IFileTemplateModuleInstance> _instances = new Dictionary<Guid, IFileTemplateModuleInstance>();

		public void Add(Guid id) {
			// Create an instance.
			IFileTemplateModuleInstance instance = Modules.GetById(id) as IFileTemplateModuleInstance;
			// Load data from user data.
			FileTemplateModuleManagement manager = Modules.GetModuleManager<IFileTemplateModuleInstance, FileTemplateModuleManagement>();
			manager.LoadTemplateData(instance);
			// Reference the instance.
			_instances[id] = instance;
		}

		public IFileTemplateModuleInstance Get(Guid id) {
			IFileTemplateModuleInstance instance = null;
			_instances.TryGetValue(id, out instance);
			return instance;
		}

		public IFileTemplateModuleInstance[] GetAll() {
			return _instances.Values.ToArray();
		}

		public void Remove(Guid id) {
			_instances.Remove(id);
		}

		object IModuleRepository.Get(Guid id) {
			return Get(id);
		}

		object[] IModuleRepository.GetAll() {
			return GetAll();
		}
	}
}
