using System;
using System.Collections.Generic;

namespace Vixen.Module.Service {
	abstract public class ServiceModuleDescriptorBase : ModuleDescriptorBase, IServiceModuleDescriptor, IEqualityComparer<IServiceModuleDescriptor>, IEquatable<IServiceModuleDescriptor>, IEqualityComparer<ServiceModuleDescriptorBase>, IEquatable<ServiceModuleDescriptorBase> {
		abstract public override string TypeName { get; }

		abstract public override Guid TypeId { get; }

		abstract public override string Author { get; }

		abstract public override string Description { get; }

		abstract public override string Version { get; }

		// Service module types can't have instance data classes; they're singletons, so don't let anyone set one.
		sealed public override Type ModuleDataClass {
			get { return null; }
		}

		public bool Equals(IServiceModuleDescriptor x, IServiceModuleDescriptor y) {
			return base.Equals(x, y);
		}

		public int GetHashCode(IServiceModuleDescriptor obj) {
			return base.GetHashCode();
		}

		public bool Equals(IServiceModuleDescriptor other) {
			return base.Equals(other);
		}

		public bool Equals(ServiceModuleDescriptorBase x, ServiceModuleDescriptorBase y) {
			return Equals((IServiceModuleDescriptor)x, (IServiceModuleDescriptor)y);
		}

		public int GetHashCode(ServiceModuleDescriptorBase obj) {
			return GetHashCode((IServiceModuleDescriptor)obj);
		}

		public bool Equals(ServiceModuleDescriptorBase other) {
			return Equals((IServiceModuleDescriptor)other);
		}
	}
}
