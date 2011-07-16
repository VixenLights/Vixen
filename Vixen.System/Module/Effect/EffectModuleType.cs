using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Common;
using Vixen.Sys;

namespace Vixen.Module.Effect {
	class EffectModuleType : IModuleLoadNotification {
		public void ModuleLoaded(IModuleDescriptor descriptor) {
			Modules.ModuleRepository.AddEffect(descriptor.TypeId);
		}

		public void ModuleUnloading(IModuleDescriptor descriptor) {
			Modules.ModuleRepository.RemoveEffect(descriptor.TypeId);
		}
	}
}
