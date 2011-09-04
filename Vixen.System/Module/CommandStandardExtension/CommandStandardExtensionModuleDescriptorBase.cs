using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.Module.CommandStandardExtension {
	abstract public class CommandStandardExtensionModuleDescriptorBase : ModuleDescriptorBase, ICommandStandardExtensionModuleDescriptor, IEqualityComparer<ICommandStandardExtensionModuleDescriptor>, IEquatable<ICommandStandardExtensionModuleDescriptor>, IEqualityComparer<CommandStandardExtensionModuleDescriptorBase>, IEquatable<CommandStandardExtensionModuleDescriptorBase> {
		abstract public override string TypeName { get; }

		abstract public override Guid TypeId { get; }

		abstract public override string Author { get; }

		abstract public override string Description { get; }

		abstract public override string Version { get; }

		public bool Equals(ICommandStandardExtensionModuleDescriptor x, ICommandStandardExtensionModuleDescriptor y) {
			return base.Equals(x, y);
		}

		public int GetHashCode(ICommandStandardExtensionModuleDescriptor obj) {
			return base.GetHashCode();
		}

		public bool Equals(ICommandStandardExtensionModuleDescriptor other) {
			return base.Equals(other);
		}

		public bool Equals(CommandStandardExtensionModuleDescriptorBase x, CommandStandardExtensionModuleDescriptorBase y) {
			return Equals(x as ICommandStandardExtensionModuleDescriptor, y as ICommandStandardExtensionModuleDescriptor);
		}

		public int GetHashCode(CommandStandardExtensionModuleDescriptorBase obj) {
			return GetHashCode(obj as ICommandStandardExtensionModuleDescriptor);
		}

		public bool Equals(CommandStandardExtensionModuleDescriptorBase other) {
			return Equals(other as ICommandStandardExtensionModuleDescriptor);
		}
	}
}
