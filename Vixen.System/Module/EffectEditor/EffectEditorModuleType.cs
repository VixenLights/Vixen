using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;

namespace Vixen.Module.EffectEditor {
	class EffectEditorModuleType : IModuleLoadNotification {
		public void ModuleLoaded(IModuleDescriptor descriptor) {
			Modules.ModuleRepository.AddEffectEditor(descriptor.TypeId);
		}

		public void ModuleUnloading(IModuleDescriptor descriptor) {
			Modules.ModuleRepository.RemoveEffectEditor(descriptor.TypeId);
		}
	}
}
