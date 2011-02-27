using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;
using CommandStandard;

namespace Vixen.Module.CommandEditor {
	class CommandEditorModuleRepository : IModuleRepository<ICommandEditorModuleInstance> {
		// Command handler module type id : command editor
		private Dictionary<Guid, ICommandEditorModuleInstance> _commandEditorCommandIndex = new Dictionary<Guid, ICommandEditorModuleInstance>();
		// Command signature : command editor
		private Dictionary<int, ICommandEditorModuleInstance> _commandEditorSignatureIndex = new Dictionary<int, ICommandEditorModuleInstance>();
		// Distinct collection of all.
		private List<ICommandEditorModuleInstance> _commandEditors = new List<ICommandEditorModuleInstance>();

		public ICommandEditorModuleInstance GetByCommandSpecId(Guid moduleId) {
			ICommandEditorModuleInstance instance;
			_commandEditorCommandIndex.TryGetValue(moduleId, out instance);
			return instance;
		}

		public ICommandEditorModuleInstance Get(CommandParameterSpecification[] commandSignature) {
			int key = _GetSignatureKey(commandSignature);
			ICommandEditorModuleInstance instance;
			_commandEditorSignatureIndex.TryGetValue(key, out instance);
			return instance;
		}

		public ICommandEditorModuleInstance Get(Guid id) {
			return _commandEditors.FirstOrDefault(x => x.TypeId == id);
		}

		public ICommandEditorModuleInstance[] GetAll() {
			return _commandEditors.ToArray();
		}

		public void Add(Guid id) {
			// Create an instance.
			ICommandEditorModuleInstance instance = Modules.GetById(id) as ICommandEditorModuleInstance;
			// Add to the collection.
			_commandEditors.Add(instance);
			// Add to the command-specific index, if appropriate.
			if(instance.CommandSpecTypeId != Guid.Empty) {
				_commandEditorCommandIndex[instance.CommandSpecTypeId] = instance;
			}
			// Add to the signature index, if appropriate.
			if(instance.CommandSignature != null) {
				_commandEditorSignatureIndex[_GetSignatureKey(instance.CommandSignature)] = instance;
			}
		}

		object IModuleRepository.Get(Guid id) {
			return Get(id);
		}

		object[] IModuleRepository.GetAll() {
			return GetAll();
		}

		public void Remove(Guid id) {
			ICommandEditorModuleInstance instance = _commandEditors.FirstOrDefault(x => x.TypeId == id);
			if(instance != null) {
				// Remove from the name index.
				_commandEditorCommandIndex.Remove(instance.CommandSpecTypeId);
				// Remove from the signature index.
				_commandEditorSignatureIndex.Remove(_GetSignatureKey(instance.CommandSignature));
				// Remove from the collection.
				_commandEditors.Remove(instance);
			}
		}


		private int _GetSignatureKey(CommandParameterSpecification[] commandSignature) {
			if(commandSignature == null) return 0;
			// Key will be a hash of the concatenation of the parameter type names.
			return commandSignature.Aggregate("", (str, spec) => str + spec.Type.Name, str => str.GetHashCode());
		}
	}
}
