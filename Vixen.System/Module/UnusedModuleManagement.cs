using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Common;
using Vixen.Sys;

namespace Vixen.Module {
	public class UnusedModuleManagement<T> : IModuleManagement<T>
		where T : class, IModuleInstance {
		public T Get(Guid id) {
			return Modules.GetById(id) as T;
		}

		object IModuleManagement.Get(Guid id) {
			return Get(id);
		}

		public T[] GetAll() {
			return Modules.GetModuleDescriptors<T>().Select(x => Get(x.TypeId)).ToArray();
		}

		object[] IModuleManagement.GetAll() {
			return GetAll();
		}

		public T Clone(T instance) {
			// Can't clone something we know nothing about.
			return null;
		}

		object IModuleManagement.Clone(object instance) {
			return Clone(instance as T);
		}
	}
}
