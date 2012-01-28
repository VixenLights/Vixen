using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.Module.Intent {
	abstract public class IntentModuleDescriptorBase : ModuleDescriptorBase, IIntentModuleDescriptor, IEqualityComparer<IIntentModuleDescriptor>, IEquatable<IIntentModuleDescriptor>, IEqualityComparer<IntentModuleDescriptorBase>, IEquatable<IntentModuleDescriptorBase> {
		abstract public override string TypeName { get; }

		abstract public override Guid TypeId { get; }

		abstract public override Type ModuleClass { get; }

		abstract public override string Author { get; }

		abstract public override string Description { get; }

		abstract public override string Version { get; }

		public bool Equals(IIntentModuleDescriptor x, IIntentModuleDescriptor y) {
			return base.Equals(x, y);
		}

		public int GetHashCode(IIntentModuleDescriptor obj) {
			return base.GetHashCode();
		}

		public bool Equals(IIntentModuleDescriptor other) {
			return base.Equals(other);
		}

		public bool Equals(IntentModuleDescriptorBase x, IntentModuleDescriptorBase y) {
			return Equals(x as IIntentModuleDescriptor, y as IIntentModuleDescriptor);
		}

		public int GetHashCode(IntentModuleDescriptorBase obj) {
			return GetHashCode(obj as IIntentModuleDescriptor);
		}

		public bool Equals(IntentModuleDescriptorBase other) {
			return Equals(other as IIntentModuleDescriptor);
		}
	}
}
