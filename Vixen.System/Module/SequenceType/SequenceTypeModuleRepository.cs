using System;
using Vixen.Sys;

namespace Vixen.Module.SequenceType
{
	internal class SequenceTypeModuleRepository : IModuleRepository<ISequenceTypeModuleInstance>
	{
		private SingletonRepository<ISequenceTypeModuleInstance> _repository;

		public SequenceTypeModuleRepository()
		{
			_repository = new SingletonRepository<ISequenceTypeModuleInstance>();
		}

		public void Add(Guid id)
		{
			// Create a singleton instance.
			ISequenceTypeModuleInstance instance = (ISequenceTypeModuleInstance) Modules.GetById(id);
			// Add it to the repository.
			_repository.Add(instance);
		}

		public ISequenceTypeModuleInstance Get(Guid id)
		{
			return _repository.Get(id);
		}

		public ISequenceTypeModuleInstance[] GetAll()
		{
			return _repository.GetAll();
		}

		public void Remove(Guid id)
		{
			_repository.Remove(id);
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