using System;
using System.Collections.Generic;

namespace Vixen.Module.OutputFilter {
	abstract public class OutputFilterModuleDescriptorBase : ModuleDescriptorBase, IOutputFilterModuleDescriptor, IEqualityComparer<IOutputFilterModuleDescriptor>, IEquatable<IOutputFilterModuleDescriptor>, IEqualityComparer<OutputFilterModuleDescriptorBase>, IEquatable<OutputFilterModuleDescriptorBase> {
		abstract public override string TypeName { get; }

		abstract public override Guid TypeId { get; }

		abstract public override Type ModuleClass { get; }

		abstract public override string Author { get; }

		abstract public override string Description { get; }

		abstract public override string Version { get; }

		public bool Equals(IOutputFilterModuleDescriptor x, IOutputFilterModuleDescriptor y) {
			return base.Equals(x, y);
		}

		public int GetHashCode(IOutputFilterModuleDescriptor obj) {
			return base.GetHashCode();
		}

		public bool Equals(IOutputFilterModuleDescriptor other) {
			return base.Equals(other);
		}

		public bool Equals(OutputFilterModuleDescriptorBase x, OutputFilterModuleDescriptorBase y) {
			return Equals(x as IOutputFilterModuleDescriptor, y as IOutputFilterModuleDescriptor);
		}

		public int GetHashCode(OutputFilterModuleDescriptorBase obj) {
			return GetHashCode(obj as IOutputFilterModuleDescriptor);
		}

		public bool Equals(OutputFilterModuleDescriptorBase other) {
			return Equals(other as IOutputFilterModuleDescriptor);
		}
	}
}
