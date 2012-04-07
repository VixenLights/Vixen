using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.Module.Controller {
	abstract public class ControllerModuleDescriptorBase : ModuleDescriptorBase, IControllerModuleDescriptor, IEqualityComparer<IControllerModuleDescriptor>, IEquatable<IControllerModuleDescriptor>, IEqualityComparer<ControllerModuleDescriptorBase>, IEquatable<ControllerModuleDescriptorBase> {
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

		public bool Equals(IControllerModuleDescriptor x, IControllerModuleDescriptor y) {
			return base.Equals(x, y);
		}

		public int GetHashCode(IControllerModuleDescriptor obj) {
			return base.GetHashCode();
		}

		public bool Equals(IControllerModuleDescriptor other) {
			return base.Equals(other);
		}

		public bool Equals(ControllerModuleDescriptorBase x, ControllerModuleDescriptorBase y) {
			return Equals(x as IControllerModuleDescriptor, y as IControllerModuleDescriptor);
		}

		public int GetHashCode(ControllerModuleDescriptorBase obj) {
			return GetHashCode(obj as IControllerModuleDescriptor);
		}

		public bool Equals(ControllerModuleDescriptorBase other) {
			return Equals(other is IControllerModuleDescriptor);
		}
	}
}
