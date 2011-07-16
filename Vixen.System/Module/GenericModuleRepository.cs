using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Common;
using Vixen.Sys;

namespace Vixen.Module {
	class GenericModuleRepository<T> : IModuleRepository<T>
		where T : class, IModuleInstance {
		virtual public void Add(Guid id) { }

		virtual public T Get(Guid id) {
			return Modules.GetById(id) as T;
		}

		virtual public T[] GetAll() {
			return Modules.GetModuleDescriptors<T>().Select(x => Get(x.TypeId)).ToArray();
		}

		virtual public void Remove(Guid id) { }

		object IModuleRepository.Get(Guid id) {
			return Get(id);
		}

		object[] IModuleRepository.GetAll() {
			return GetAll();
		}
	}
}
