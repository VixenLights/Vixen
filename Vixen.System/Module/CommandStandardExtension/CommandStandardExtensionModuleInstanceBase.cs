using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;
using Vixen.Commands;

namespace Vixen.Module.CommandStandardExtension {
	abstract public class CommandStandardExtensionModuleInstanceBase : ModuleInstanceBase, ICommandStandardExtensionModuleInstance, IEqualityComparer<ICommandStandardExtensionModuleInstance>, IEquatable<ICommandStandardExtensionModuleInstance>, IEqualityComparer<CommandStandardExtensionModuleInstanceBase>, IEquatable<CommandStandardExtensionModuleInstanceBase> {
		private CommandParameterSignature _noParameters = new CommandParameterSignature();

		public string Name {
			get { return (Descriptor as ICommandStandardExtensionModuleDescriptor).CommandName; }
		}

		virtual public byte CommandPlatform {
			get { return (Descriptor as ICommandStandardExtensionModuleDescriptor).CommandPlatform; }
		}

		virtual public byte CommandIndex {
			get { return (Descriptor as ICommandStandardExtensionModuleDescriptor).CommandIndex; }
		}

		abstract public Command GetCommand();

		virtual public CommandParameterSignature Parameters {
			get { return _noParameters; }
		}

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
