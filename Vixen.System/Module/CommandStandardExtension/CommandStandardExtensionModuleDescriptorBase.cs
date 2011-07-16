using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.Module.CommandStandardExtension {
	abstract public class CommandStandardExtensionModuleDescriptorBase : ModuleDescriptorBase, ICommandStandardExtensionModuleDescriptor, IEqualityComparer<ICommandStandardExtensionModuleDescriptor> {
		abstract public override string TypeName { get; }

		abstract public override Guid TypeId { get; }

		abstract public override Type ModuleClass { get; }

		abstract public override Type ModuleDataClass { get; }

		abstract public override string Author { get; }

		abstract public override string Description { get; }

		abstract public override string Version { get; }

		public bool Equals(ICommandStandardExtensionModuleDescriptor x, ICommandStandardExtensionModuleDescriptor y) {
			return base.Equals(x, y);
		}

		public int GetHashCode(ICommandStandardExtensionModuleDescriptor obj) {
			return base.GetHashCode();
		}
	}
}
