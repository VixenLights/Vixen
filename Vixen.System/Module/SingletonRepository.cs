using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;

namespace Vixen.Module {
	class SingletonRepository<T>
		where T : IModuleInstance {
		private Dictionary<Guid, T> _instances = new Dictionary<Guid, T>();

		public void Add(T instance) {
			_instances[instance.Descriptor.TypeId] = instance;
		}

		public T Get(Guid id) {
			T instance;
			_instances.TryGetValue(id, out instance);
			return instance;
		}

		public T[] GetAll() {
			return _instances.Values.ToArray();
		}

		public bool Remove(Guid id) {
			return _instances.Remove(id);
		}
	}
}
