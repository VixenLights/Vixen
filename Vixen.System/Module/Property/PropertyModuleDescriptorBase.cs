using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.Module.Property
{
	public abstract class PropertyModuleDescriptorBase : ModuleDescriptorBase, IPropertyModuleDescriptor,
	                                                     IEqualityComparer<IPropertyModuleDescriptor>,
	                                                     IEquatable<IPropertyModuleDescriptor>,
	                                                     IEqualityComparer<PropertyModuleDescriptorBase>,
	                                                     IEquatable<PropertyModuleDescriptorBase>
	{
		public abstract override string TypeName { get; }

		public abstract override Guid TypeId { get; }

		public abstract override Type ModuleClass { get; }

		public abstract override string Author { get; }

		public abstract override string Description { get; }

		public abstract override string Version { get; }

		public bool Equals(IPropertyModuleDescriptor x, IPropertyModuleDescriptor y)
		{
			return base.Equals(x, y);
		}

		public int GetHashCode(IPropertyModuleDescriptor obj)
		{
			return base.GetHashCode();
		}

		public bool Equals(IPropertyModuleDescriptor other)
		{
			return base.Equals(other);
		}

		public bool Equals(PropertyModuleDescriptorBase x, PropertyModuleDescriptorBase y)
		{
			return Equals(x as IPropertyModuleDescriptor, y as IPropertyModuleDescriptor);
		}

		public int GetHashCode(PropertyModuleDescriptorBase obj)
		{
			return GetHashCode(obj as IPropertyModuleDescriptor);
		}

		public bool Equals(PropertyModuleDescriptorBase other)
		{
			return Equals(other as IPropertyModuleDescriptor);
		}
	}
}