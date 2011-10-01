using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;

namespace Vixen.Module.CommandStandardExtension {
	class CommandStandardExtensionModuleRepository : GenericModuleRepository<ICommandStandardExtensionModuleInstance> {
		private HashSet<CommandStandardLoadKey> _commandKeys = new HashSet<CommandStandardLoadKey>();
		private Dictionary<CommandStandardRetrieveKey, Guid> _lookup = new Dictionary<CommandStandardRetrieveKey, Guid>();

		public override void Add(Guid id) {
			ICommandStandardExtensionModuleDescriptor descriptor = Modules.GetDescriptorById<ICommandStandardExtensionModuleDescriptor>(id);
			CommandStandardLoadKey loadKey = new CommandStandardLoadKey(descriptor.CommandName, descriptor.CommandPlatform, descriptor.CommandIndex);
			if(_commandKeys.Add(loadKey)) {
				CommandStandardRetrieveKey retrieveKey = new CommandStandardRetrieveKey(descriptor.CommandName, descriptor.CommandPlatform);
				_lookup[retrieveKey] = id;
			} else {
				VixenSystem.Logging.Error("Command standard extension from module " + descriptor.TypeName + " duplicates a key already used and therefore was not loaded.");
			}
		}

		public override void Remove(Guid id) {
			ICommandStandardExtensionModuleDescriptor descriptor = Modules.GetDescriptorById<ICommandStandardExtensionModuleDescriptor>(id);
			CommandStandardLoadKey loadKey = new CommandStandardLoadKey(descriptor.CommandName, descriptor.CommandPlatform, descriptor.CommandIndex);
			if(_commandKeys.Remove(loadKey)) {
				CommandStandardRetrieveKey retrieveKey = new CommandStandardRetrieveKey(descriptor.CommandName, descriptor.CommandPlatform);
				_lookup.Remove(retrieveKey);
			}
		}

		public ICommandStandardExtensionModuleInstance Get(byte platformValue, string extensionName) {
			Guid id;
			CommandStandardRetrieveKey retrieveKey = new CommandStandardRetrieveKey(extensionName, platformValue);
			_lookup.TryGetValue(retrieveKey, out id);
			return Get(id);
		}

		// Keyed on:
		// Name - because that's how it's retrieved within the context of a platform.
		// Platform - because custom commands are per-platform
		// Command index - to insure that all custom commands within a platform have unique identifiers
		class CommandStandardLoadKey : Tuple<string, byte, byte> {
			public CommandStandardLoadKey(string name, byte platformValue, byte commandIndexValue)
				: base(name, platformValue, commandIndexValue) {
			}
		}

		// Keyed on:
		// Name
		// Platform
		// (Command index is not needed for retrieval, only to ensure uniqueness when loading)
		class CommandStandardRetrieveKey : Tuple<string, byte> {
			public CommandStandardRetrieveKey(string name, byte platformValue)
				: base(name, platformValue) {
			}
		}
	}
}
