using System;
using System.Collections.Generic;
using System.Linq;
using Vixen.Sys;

namespace Vixen.Module.EffectEditor {
	class EffectEditorModuleRepository : IModuleRepository<IEffectEditorModuleInstance> {
		// Effect type id : effect editor
		private Dictionary<Guid, IEffectEditorModuleInstance> _effectEditorEffectIndex = new Dictionary<Guid, IEffectEditorModuleInstance>();
		// Effect signature : effect editor
		private Dictionary<int, IEffectEditorModuleInstance> _effectEditorSignatureIndex = new Dictionary<int, IEffectEditorModuleInstance>();
		// Singleton collection.
		private List<IEffectEditorModuleInstance> _effectEditors = new List<IEffectEditorModuleInstance>();

		public IEffectEditorModuleInstance GetByEffectId(Guid moduleId) {
			IEffectEditorModuleInstance instance;
			_effectEditorEffectIndex.TryGetValue(moduleId, out instance);
			return instance;
		}

		public IEffectEditorModuleInstance Get(IEnumerable<Type> signature) {
			int key = _GetSignatureKey(signature);
			IEffectEditorModuleInstance instance;
			_effectEditorSignatureIndex.TryGetValue(key, out instance);
			return instance;
		}

		public IEffectEditorModuleInstance Get(Type type) {
			int key = _GetSignatureKey(type.AsEnumerable());
			IEffectEditorModuleInstance instance;
			_effectEditorSignatureIndex.TryGetValue(key, out instance);
			return instance;
		}

		public IEffectEditorModuleInstance Get(Guid id) {
			return _effectEditors.FirstOrDefault(x => x.Descriptor.TypeId == id);
		}

		public IEffectEditorModuleInstance[] GetAll() {
			return _effectEditors.ToArray();
		}

		public void Add(Guid id) {
			// Create an instance.
			IEffectEditorModuleInstance instance = (IEffectEditorModuleInstance)Modules.GetById(id);
			// Add to the collection.
			_effectEditors.Add(instance);
			// Add to the effect-specific index, if appropriate.
			if(instance.EffectTypeId != Guid.Empty) {
				_effectEditorEffectIndex[instance.EffectTypeId] = instance;
			}
			// Add to the signature index, if appropriate.
			if(instance.ParameterSignature != null) {
				_effectEditorSignatureIndex[_GetSignatureKey(instance.ParameterSignature)] = instance;
			}
		}

		object IModuleRepository.Get(Guid id) {
			return Get(id);
		}

		object[] IModuleRepository.GetAll() {
			return GetAll();
		}

		public void Remove(Guid id) {
			IEffectEditorModuleInstance instance = _effectEditors.FirstOrDefault(x => x.Descriptor.TypeId == id);
			if(instance != null) {
				// Remove from the name index.
				_effectEditorEffectIndex.Remove(instance.EffectTypeId);
				// Remove from the signature index.
				_effectEditorSignatureIndex.Remove(_GetSignatureKey(instance.ParameterSignature));
				// Remove from the collection.
				_effectEditors.Remove(instance);
			}
		}


		private int _GetSignatureKey(IEnumerable<Type> signature) {
			if(signature == null) return 0;
			// Key will be a hash of the concatenation of the parameter type names.
			return signature.Aggregate("", (str, type) => str + type.Name, str => str.GetHashCode());
		}
	}
}
