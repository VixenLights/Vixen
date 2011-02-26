using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Vixen.Module {
	interface IModuleRepository {
		void Add(Guid id);
		object Get(Guid id);
		object[] GetAll();
		void Remove(Guid id);
	}

	interface IModuleRepository<T> : IModuleRepository
		where T : class {
		new T Get(Guid id);
		new T[] GetAll();
	}
}
