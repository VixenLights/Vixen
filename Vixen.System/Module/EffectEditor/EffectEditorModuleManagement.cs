using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;
using Vixen.Module.Effect;
using Vixen.Commands;

namespace Vixen.Module.EffectEditor {
	class EffectEditorModuleManagement : GenericModuleManagement<IEffectEditorModuleInstance> {
		public IEnumerable<IEffectEditorControl> GetEffectEditors(Guid effectId) {
			IEffectModuleDescriptor descriptor = Modules.GetDescriptorById<IEffectModuleDescriptor>(effectId);

			// 1. Is there an editor for this specific effect by id?
			// 2. Is there an editor for this specific signature?
			// 3. Try to get a collection of editors, one for each parameter type.
			IEnumerable<IEffectEditorControl> editorControls =
				_GetEditorByEffect(descriptor) ??
				_GetEditorBySignature(descriptor) ??
				_GetEditorsByParameter(descriptor);

			// May be null if nothing qualifies.
			return editorControls;
		}

		private IEnumerable<IEffectEditorControl> _GetEditorByEffect(IEffectModuleDescriptor descriptor) {
			// Need the type-specific repository reference, doing more than basic
			// repository operations.
			EffectEditorModuleRepository repository = Modules.GetRepository<IEffectEditorModuleInstance, EffectEditorModuleRepository>();
			IEffectEditorModuleInstance instance = repository.GetByEffectId(descriptor.TypeId);
			return (instance != null) ? instance.CreateEditorControl().AsEnumerable() : null;
		}

		private IEnumerable<IEffectEditorControl> _GetEditorBySignature(IEffectModuleDescriptor descriptor) {
			EffectEditorModuleRepository repository = Modules.GetRepository<IEffectEditorModuleInstance, EffectEditorModuleRepository>();
			IEffectEditorModuleInstance instance = repository.Get(descriptor.Parameters);
			return (instance != null) ? instance.CreateEditorControl().AsEnumerable() : null;
		}

		private IEnumerable<IEffectEditorControl> _GetEditorsByParameter(IEffectModuleDescriptor descriptor) {
			EffectEditorModuleRepository repository = Modules.GetRepository<IEffectEditorModuleInstance, EffectEditorModuleRepository>();
			IEnumerable<IEffectEditorModuleInstance> instances = descriptor.Parameters.Select(repository.Get);
			if(!instances.Any(x => x == null)) {
				return instances.Select(x => x.CreateEditorControl());
			}
			return null;
		}
	}
}
