using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommandStandard;
using Vixen.Sys;

namespace Vixen.Module.Effect {
	class EffectModuleManagement : GenericModuleManagement<IEffectModuleInstance> {
		public IEffectModuleInstance Get(string commandName) {
			// Need the type-specific repository.
			EffectModuleRepository repository = Modules.GetRepository<IEffectModuleInstance, EffectModuleRepository>();
			return repository.Get(commandName);
		}
	}
	//class EffectModuleManagement : IModuleManagement<IEffectModuleInstance> {
	//    public IEffectModuleInstance Get(string commandName) {
	//        // Need the type-specific repository.
	//        EffectModuleRepository repository = Modules.GetModuleRepository<IEffectModuleInstance, EffectModuleRepository>();
	//        return repository.Get(commandName);
	//    }

	//    public IEffectModuleInstance Get(Guid id) {
	//        return Modules.ModuleRepository.GetEffect(id);
	//    }

	//    public IEffectModuleInstance[] GetAll() {
	//        return Modules.ModuleRepository.GetAllEffect();
	//    }

	//    public IEffectModuleInstance Clone(IEffectModuleInstance instance) {
	//        // These are singletons.
	//        return null;
	//    }

	//    object IModuleManagement.Get(Guid id) {
	//        return Get(id);
	//    }

	//    object[] IModuleManagement.GetAll() {
	//        return GetAll();
	//    }

	//    public object Clone(object instance) {
	//        return Clone(instance as IEffectModuleInstance);
	//    }
	//}
}
