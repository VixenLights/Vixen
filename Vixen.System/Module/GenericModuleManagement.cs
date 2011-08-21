using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;

namespace Vixen.Module {
	class GenericModuleManagement<T> : IModuleManagement<T>
		where T : class, IModuleInstance {
		virtual public T Get(Guid id) {
			return Modules.GetImplementation<T>().Repository.Get(id) as T;
		}

		object IModuleManagement.Get(Guid id) {
			return Get(id);
		}

		virtual public T[] GetAll() {
			return Modules.GetModuleDescriptors<T>().Select(x => Get(x.TypeId)).ToArray();
		}

		object[] IModuleManagement.GetAll() {
			return GetAll();
		}

		virtual public T Clone(T instance) {
			// Can't clone something we know nothing about.
			return null;
		}

		object IModuleManagement.Clone(object instance) {
			return Clone(instance as T);
		}
	}
}
