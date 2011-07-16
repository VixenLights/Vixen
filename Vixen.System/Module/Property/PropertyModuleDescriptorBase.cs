using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.Module.Property {
	abstract public class PropertyModuleDescriptorBase : ModuleDescriptorBase, IPropertyModuleDescriptor, IEqualityComparer<IPropertyModuleDescriptor> {
		abstract public override string TypeName { get; }

		abstract public override Guid TypeId { get; }

		abstract public override Type ModuleClass { get; }

		abstract public override Type ModuleDataClass { get; }

		abstract public override string Author { get; }

		abstract public override string Description { get; }

		abstract public override string Version { get; }

		public bool Equals(IPropertyModuleDescriptor x, IPropertyModuleDescriptor y) {
			return base.Equals(x, y);
		}

		public int GetHashCode(IPropertyModuleDescriptor obj) {
			return base.GetHashCode();
		}
	}
}
