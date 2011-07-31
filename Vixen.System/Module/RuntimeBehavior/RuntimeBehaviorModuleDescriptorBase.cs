using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.Module.RuntimeBehavior {
	abstract public class RuntimeBehaviorModuleDescriptorBase : ModuleDescriptorBase, IRuntimeBehaviorModuleDescriptor, IEqualityComparer<IRuntimeBehaviorModuleDescriptor>, IEquatable<IRuntimeBehaviorModuleDescriptor>, IEqualityComparer<RuntimeBehaviorModuleDescriptorBase>, IEquatable<RuntimeBehaviorModuleDescriptorBase> {
		abstract public override string TypeName { get; }

		abstract public override Guid TypeId { get; }

		abstract public override Type ModuleClass { get; }

		abstract public override Type ModuleDataClass { get; }

		abstract public override string Author { get; }

		abstract public override string Description { get; }

		abstract public override string Version { get; }

		public bool Equals(IRuntimeBehaviorModuleDescriptor x, IRuntimeBehaviorModuleDescriptor y) {
			return base.Equals(x, y);
		}

		public int GetHashCode(IRuntimeBehaviorModuleDescriptor obj) {
			return base.GetHashCode();
		}

		public bool Equals(IRuntimeBehaviorModuleDescriptor other) {
			return base.Equals(other);
		}

		public bool Equals(RuntimeBehaviorModuleDescriptorBase x, RuntimeBehaviorModuleDescriptorBase y) {
			return Equals(x as IRuntimeBehaviorModuleDescriptor, y as IRuntimeBehaviorModuleDescriptor);
		}

		public int GetHashCode(RuntimeBehaviorModuleDescriptorBase obj) {
			return GetHashCode(obj as IRuntimeBehaviorModuleDescriptor);
		}

		public bool Equals(RuntimeBehaviorModuleDescriptorBase other) {
			return Equals(other as IRuntimeBehaviorModuleDescriptor);
		}
	}
}
