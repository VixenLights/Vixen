using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.Module.Trigger {
	abstract public class TriggerModuleDescriptorBase : ModuleDescriptorBase, ITriggerModuleDescriptor, IEqualityComparer<ITriggerModuleDescriptor>, IEquatable<ITriggerModuleDescriptor>, IEqualityComparer<TriggerModuleDescriptorBase>, IEquatable<TriggerModuleDescriptorBase> {
		abstract public override string TypeName { get; }

		abstract public override Guid TypeId { get; }

		abstract public override Type ModuleClass { get; }

		abstract public override string Author { get; }

		abstract public override string Description { get; }

		abstract public override string Version { get; }

		public bool Equals(ITriggerModuleDescriptor x, ITriggerModuleDescriptor y) {
			return base.Equals(x, y);
		}

		public int GetHashCode(ITriggerModuleDescriptor obj) {
			return base.GetHashCode();
		}

		public bool Equals(ITriggerModuleDescriptor other) {
			return base.Equals(other);
		}

		public bool Equals(TriggerModuleDescriptorBase x, TriggerModuleDescriptorBase y) {
			return Equals(x as ITriggerModuleDescriptor, y as ITriggerModuleDescriptor);
		}

		public int GetHashCode(TriggerModuleDescriptorBase obj) {
			return GetHashCode(obj as ITriggerModuleDescriptor);
		}

		public bool Equals(TriggerModuleDescriptorBase other) {
			return Equals(other as ITriggerModuleDescriptor);
		}
	}
}
