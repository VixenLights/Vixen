using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Common;
using Vixen.Sys;

namespace Vixen.Module.Effect {
	class EffectModuleRepository : IModuleRepository<IEffectModuleInstance> {
		// Effect name : command handler instance
		private Dictionary<string, IEffectModuleInstance> _instances = new Dictionary<string, IEffectModuleInstance>();

		public IEffectModuleInstance Get(Guid id) {
			return _instances.Values.FirstOrDefault(x => x.TypeId == id);
		}

		public IEffectModuleInstance[] GetAll() {
			return _instances.Values.ToArray();
		}

		public IEffectModuleInstance Get(string commandName) {
			IEffectModuleInstance instance;
			_instances.TryGetValue(commandName, out instance);
			return instance;
		}

		public void Add(Guid id) {
			// Get the module descriptor.
			IEffectModuleDescriptor descriptor = Modules.GetDescriptorById<IEffectModuleDescriptor>(id);
			// Create an singleton instance.
			IEffectModuleInstance instance = Modules.GetById(id) as IEffectModuleInstance;
			// Add the instance to the dictionary.
			_instances[instance.EffectName] = instance;
		}

		object IModuleRepository.Get(Guid id) {
			return Get(id);
		}

		object[] IModuleRepository.GetAll() {
			return GetAll();
		}

		public void Remove(Guid id) {
			// Remove the handler instance.
			_instances.Remove(_instances.Values.FirstOrDefault(x => x.TypeId == id).EffectName);
		}
	}
}
