using System;
using System.Linq;
using Vixen.Sys;

namespace Vixen.Module
{
	// The generic repository isn't actually a repository.  Any instances retrieved
	// from it are created new.  Subclasses have to implement their proprietary
	// repository scheme to fits their specific needs.
	internal class GenericModuleRepository<T> : IModuleRepository<T>
		where T : class, IModuleInstance
	{
		public virtual void Add(Guid id)
		{
		}

		public virtual T Get(Guid id)
		{
			return Modules.GetById(id) as T;
		}

		public virtual T[] GetAll()
		{
			return Modules.GetDescriptors<T>().Select(x => Get(x.TypeId)).ToArray();
		}

		public virtual void Remove(Guid id)
		{
		}

		object IModuleRepository.Get(Guid id)
		{
			return Get(id);
		}

		object[] IModuleRepository.GetAll()
		{
			return GetAll();
		}
	}
}