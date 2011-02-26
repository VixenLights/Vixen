using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.Module {
	public interface IModuleManagement {
		object Get(Guid id);
		object[] GetAll();
		object Clone(object instance);
	}

	public interface IModuleManagement<T> : IModuleManagement
		where T : class {
		new T Get(Guid id);
		new T[] GetAll();
		T Clone(T instance);
	}
}
