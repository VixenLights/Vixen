using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.Module.Property {
	abstract public class PropertyModuleDescriptorBase : ModuleDescriptorBase, IPropertyModuleDescriptor, IEqualityComparer<IPropertyModuleDescriptor>, IEquatable<IPropertyModuleDescriptor>, IEqualityComparer<PropertyModuleDescriptorBase>, IEquatable<PropertyModuleDescriptorBase> {
		abstract public override string TypeName { get; }

		abstract public override Guid TypeId { get; }

		abstract public override Type ModuleClass { get; }

		abstract public override string Author { get; }

		abstract public override string Description { get; }

		abstract public override string Version { get; }

		public bool Equals(IPropertyModuleDescriptor x, IPropertyModuleDescriptor y) {
			return base.Equals(x, y);
		}

		public int GetHashCode(IPropertyModuleDescriptor obj) {
			return base.GetHashCode();
		}

		public bool Equals(IPropertyModuleDescriptor other) {
			return base.Equals(other);
		}

		public bool Equals(PropertyModuleDescriptorBase x, PropertyModuleDescriptorBase y) {
			return Equals(x as IPropertyModuleDescriptor, y as IPropertyModuleDescriptor);
		}

		public int GetHashCode(PropertyModuleDescriptorBase obj) {
			return GetHashCode(obj as IPropertyModuleDescriptor);
		}

		public bool Equals(PropertyModuleDescriptorBase other) {
			return Equals(other as IPropertyModuleDescriptor);
		}
	}
}
