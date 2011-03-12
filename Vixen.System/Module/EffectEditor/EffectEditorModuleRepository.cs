using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;
using CommandStandard;

namespace Vixen.Module.EffectEditor {
	class EffectEditorModuleRepository : IModuleRepository<IEffectEditorModuleInstance> {
		// Command handler module type id : command editor
		private Dictionary<Guid, IEffectEditorModuleInstance> _effectEditorCommandIndex = new Dictionary<Guid, IEffectEditorModuleInstance>();
		// Command signature : command editor
		private Dictionary<int, IEffectEditorModuleInstance> _effectEditorSignatureIndex = new Dictionary<int, IEffectEditorModuleInstance>();
		// Distinct collection of all.
		private List<IEffectEditorModuleInstance> _effectEditors = new List<IEffectEditorModuleInstance>();

		public IEffectEditorModuleInstance GetByEffectId(Guid moduleId) {
			IEffectEditorModuleInstance instance;
			_effectEditorCommandIndex.TryGetValue(moduleId, out instance);
			return instance;
		}

		public IEffectEditorModuleInstance Get(CommandParameterSpecification[] commandSignature) {
			int key = _GetSignatureKey(commandSignature);
			IEffectEditorModuleInstance instance;
			_effectEditorSignatureIndex.TryGetValue(key, out instance);
			return instance;
		}

		public IEffectEditorModuleInstance Get(Guid id) {
			return _effectEditors.FirstOrDefault(x => x.TypeId == id);
		}

		public IEffectEditorModuleInstance[] GetAll() {
			return _effectEditors.ToArray();
		}

		public void Add(Guid id) {
			// Create an instance.
			IEffectEditorModuleInstance instance = Modules.GetById(id) as IEffectEditorModuleInstance;
			// Add to the collection.
			_effectEditors.Add(instance);
			// Add to the command-specific index, if appropriate.
			if(instance.EffectTypeId != Guid.Empty) {
				_effectEditorCommandIndex[instance.EffectTypeId] = instance;
			}
			// Add to the signature index, if appropriate.
			if(instance.CommandSignature != null) {
				_effectEditorSignatureIndex[_GetSignatureKey(instance.CommandSignature)] = instance;
			}
		}

		object IModuleRepository.Get(Guid id) {
			return Get(id);
		}

		object[] IModuleRepository.GetAll() {
			return GetAll();
		}

		public void Remove(Guid id) {
			IEffectEditorModuleInstance instance = _effectEditors.FirstOrDefault(x => x.TypeId == id);
			if(instance != null) {
				// Remove from the name index.
				_effectEditorCommandIndex.Remove(instance.EffectTypeId);
				// Remove from the signature index.
				_effectEditorSignatureIndex.Remove(_GetSignatureKey(instance.CommandSignature));
				// Remove from the collection.
				_effectEditors.Remove(instance);
			}
		}


		private int _GetSignatureKey(CommandParameterSpecification[] commandSignature) {
			if(commandSignature == null) return 0;
			// Key will be a hash of the concatenation of the parameter type names.
			return commandSignature.Aggregate("", (str, spec) => str + spec.Type.Name, str => str.GetHashCode());
		}
	}
}
