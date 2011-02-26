using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Common;
using Vixen.Sys;

namespace Vixen.Module.CommandSpec {
	class CommandSpecModuleRepository : IModuleRepository<ICommandSpecModuleInstance> {
		// Command name : command handler instance
		private Dictionary<string, ICommandSpecModuleInstance> _instances = new Dictionary<string, ICommandSpecModuleInstance>();

		public ICommandSpecModuleInstance Get(Guid id) {
			return _instances.Values.FirstOrDefault(x => x.TypeId == id);
		}

		public ICommandSpecModuleInstance[] GetAll() {
			return _instances.Values.ToArray();
		}

		public ICommandSpecModuleInstance Get(string commandName) {
			ICommandSpecModuleInstance instance;
			_instances.TryGetValue(commandName, out instance);
			return instance;
		}

		public void Add(Guid id) {
			// Get the module descriptor.
			ICommandSpecModuleDescriptor descriptor = Modules.GetDescriptorById<ICommandSpecModuleDescriptor>(id);
			// Create an singleton instance.
			ICommandSpecModuleInstance instance = Modules.GetById(id) as ICommandSpecModuleInstance;
			// Add the instance to the dictionary.
			_instances[instance.CommandName] = instance;
		}

		object IModuleRepository.Get(Guid id) {
			return Get(id);
		}

		object[] IModuleRepository.GetAll() {
			return GetAll();
		}

		public void Remove(Guid id) {
			// Remove the handler instance.
			_instances.Remove(_instances.Values.FirstOrDefault(x => x.TypeId == id).CommandName);
		}
	}
}
