using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.Module.Timing {
	abstract public class TimingModuleDescriptorBase : ModuleDescriptorBase, ITimingModuleDescriptor, IEqualityComparer<ITimingModuleDescriptor>, IEquatable<ITimingModuleDescriptor>, IEqualityComparer<TimingModuleDescriptorBase>, IEquatable<TimingModuleDescriptorBase> {
		abstract public override string TypeName { get; }

		abstract public override Guid TypeId { get; }

		abstract public override Type ModuleClass { get; }

		abstract public override string Author { get; }

		abstract public override string Description { get; }

		abstract public override string Version { get; }

		public bool Equals(ITimingModuleDescriptor x, ITimingModuleDescriptor y) {
			return base.Equals(x, y);
		}

		public int GetHashCode(ITimingModuleDescriptor obj) {
			return base.GetHashCode();
		}

		public bool Equals(ITimingModuleDescriptor other) {
			return base.Equals(other);
		}

		public bool Equals(TimingModuleDescriptorBase x, TimingModuleDescriptorBase y) {
			return Equals(x as ITimingModuleDescriptor, y as ITimingModuleDescriptor);
		}

		public int GetHashCode(TimingModuleDescriptorBase obj) {
			return GetHashCode(obj as ITimingModuleDescriptor);
		}

		public bool Equals(TimingModuleDescriptorBase other) {
			return Equals(other as ITimingModuleDescriptor);
		}
	}
}
