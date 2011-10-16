using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;

namespace Vixen.Module.Effect {
	class EffectModuleManagement : GenericModuleManagement<IEffectModuleInstance> {
		public IEffectModuleInstance Get(string commandName) {
			// Need the type-specific repository.
			EffectModuleRepository repository = Modules.GetRepository<IEffectModuleInstance, EffectModuleRepository>();
			IEffectModuleInstance instance = repository.Get(commandName);
			// Effect parameters are stored in their data object.
			// A sequence will create a data object for an added effect, but the effect
			// may be fired live without going into a sequence.
			// So we need to create an initial data instance, if there is one.
			if(instance != null) {
				instance.ModuleData = ModuleLocalDataSet.CreateModuleDataInstance(instance);
			}
			return instance;
		}

		public override IEffectModuleInstance Get(Guid id) {
			IEffectModuleInstance instance = base.Get(id);
			if(instance != null) {
				instance.ModuleData = ModuleLocalDataSet.CreateModuleDataInstance(instance);
			}
			return instance;
		}
	}
}
