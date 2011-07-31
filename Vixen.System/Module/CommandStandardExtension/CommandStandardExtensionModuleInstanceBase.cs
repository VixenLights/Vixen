using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;
using CommandStandard;

namespace Vixen.Module.CommandStandardExtension {
	// Hate doing this since it adds no value, but it keeps with the pattern.
	abstract public class CommandStandardExtensionModuleInstanceBase : ModuleInstanceBase, ICommandStandardExtensionModuleInstance, IEqualityComparer<ICommandStandardExtensionModuleInstance>, IEquatable<ICommandStandardExtensionModuleInstance>, IEqualityComparer<CommandStandardExtensionModuleInstanceBase>, IEquatable<CommandStandardExtensionModuleInstanceBase> {
		abstract public string Name { get; }

		abstract public byte CommandPlatform { get; }

		abstract public byte CommandCategory { get; }

		abstract public byte CommandIndex { get; }

		abstract public CommandParameterSpecification[] Parameters { get; }

		public bool Equals(ICommandStandardExtensionModuleInstance x, ICommandStandardExtensionModuleInstance y) {
			return base.Equals(x, y);
		}

		public int GetHashCode(ICommandStandardExtensionModuleInstance obj) {
			return base.GetHashCode(obj);
		}

		public bool Equals(ICommandStandardExtensionModuleInstance other) {
			return base.Equals(other);
		}

		public bool Equals(CommandStandardExtensionModuleInstanceBase x, CommandStandardExtensionModuleInstanceBase y) {
			return Equals(x as ICommandStandardExtensionModuleInstance, y as ICommandStandardExtensionModuleInstance);
		}

		public int GetHashCode(CommandStandardExtensionModuleInstanceBase obj) {
			return GetHashCode(obj as ICommandStandardExtensionModuleInstance);
		}

		public bool Equals(CommandStandardExtensionModuleInstanceBase other) {
			return Equals(other as ICommandStandardExtensionModuleInstance);
		}
	}
}
