using System;
using System.Collections.Generic;

namespace Vixen.Module.SmartController {
	abstract public class SmartControllerModuleDescriptorBase : ModuleDescriptorBase, ISmartControllerModuleDescriptor, IEqualityComparer<ISmartControllerModuleDescriptor>, IEquatable<ISmartControllerModuleDescriptor>, IEqualityComparer<SmartControllerModuleDescriptorBase>, IEquatable<SmartControllerModuleDescriptorBase> {
		private const int DEFAULT_UPDATE_INTERVAL = 20;

		abstract public override string TypeName { get; }

		abstract public override Guid TypeId { get; }

		abstract public override Type ModuleClass { get; }

		abstract public override string Author { get; }

		abstract public override string Description { get; }

		abstract public override string Version { get; }

		virtual public int UpdateInterval {
			get { return DEFAULT_UPDATE_INTERVAL; }
		}

		public bool Equals(ISmartControllerModuleDescriptor x, ISmartControllerModuleDescriptor y) {
			return base.Equals(x, y);
		}

		public int GetHashCode(ISmartControllerModuleDescriptor obj) {
			return base.GetHashCode();
		}

		public bool Equals(ISmartControllerModuleDescriptor other) {
			return base.Equals(other);
		}

		public bool Equals(SmartControllerModuleDescriptorBase x, SmartControllerModuleDescriptorBase y) {
			return Equals(x as ISmartControllerModuleDescriptor, y as ISmartControllerModuleDescriptor);
		}

		public int GetHashCode(SmartControllerModuleDescriptorBase obj) {
			return GetHashCode(obj as ISmartControllerModuleDescriptor);
		}

		public bool Equals(SmartControllerModuleDescriptorBase other) {
			return Equals(other is ISmartControllerModuleDescriptor);
		}
	}
}
