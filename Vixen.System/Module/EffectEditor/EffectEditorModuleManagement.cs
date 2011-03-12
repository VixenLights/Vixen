using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;
using Vixen.Module.Effect;

namespace Vixen.Module.EffectEditor {
	class EffectEditorModuleManagement : UnusedModuleManagement<IEffectEditorModuleInstance> {
		public IEffectEditorControl GetEffectEditor(Guid effectId) {
			IEffectEditorModuleInstance instance = null;

			// Need the type-specific repository reference, doing more than basic
			// repository operations.
			EffectEditorModuleRepository repository = Server.Internal.GetModuleRepository<IEffectEditorModuleInstance, EffectEditorModuleRepository>();
			// Get the command spec descriptor.
			IEffectModuleDescriptor descriptor = Modules.GetDescriptorById<IEffectModuleDescriptor>(effectId);
			// Look up by command handler id first, then command signature.
			instance = repository.GetByEffectId(descriptor.TypeId) ?? repository.Get(descriptor.Parameters);
			if(instance != null) {
				return instance.CreateEditorControl();
			}
			return null;
		}
	}
}
