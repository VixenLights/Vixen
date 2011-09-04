using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;

namespace Vixen.Module {
	// The generic repository isn't actually a repository.  Any instances retrieved
	// from it are created new.  Subclasses have to implement their proprietary
	// repository scheme to fits their specific needs.
	class GenericModuleRepository<T> : IModuleRepository<T>
		where T : class, IModuleInstance {
		virtual public void Add(Guid id) { }

		virtual public T Get(Guid id) {
			return Modules.GetById(id) as T;
		}

		virtual public T[] GetAll() {
			return Modules.GetDescriptors<T>().Select(x => Get(x.TypeId)).ToArray();
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
