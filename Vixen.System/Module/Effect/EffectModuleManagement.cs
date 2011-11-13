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
			return instance;
		}
	}
}
