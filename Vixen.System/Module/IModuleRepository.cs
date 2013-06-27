using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Vixen.Module
{
	internal interface IModuleRepository
	{
		void Add(Guid id);
		object Get(Guid id);
		object[] GetAll();
		void Remove(Guid id);
	}

	internal interface IModuleRepository<T> : IModuleRepository
		where T : class, IModuleInstance
	{
		new T Get(Guid id);
		new T[] GetAll();
	}
}