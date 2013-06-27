using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.Module.ModuleTemplate
{
	public abstract class ModuleTemplateModuleDescriptorBase : ModuleDescriptorBase, IModuleTemplateModuleDescriptor,
	                                                           IEqualityComparer<IModuleTemplateModuleDescriptor>,
	                                                           IEquatable<IModuleTemplateModuleDescriptor>,
	                                                           IEqualityComparer<ModuleTemplateModuleDescriptorBase>,
	                                                           IEquatable<ModuleTemplateModuleDescriptorBase>
	{
		public abstract override string TypeName { get; }

		public abstract override Guid TypeId { get; }

		public abstract override Type ModuleClass { get; }

		public abstract override string Author { get; }

		public abstract override string Description { get; }

		public abstract override string Version { get; }

		public bool Equals(IModuleTemplateModuleDescriptor x, IModuleTemplateModuleDescriptor y)
		{
			return base.Equals(x, y);
		}

		public int GetHashCode(IModuleTemplateModuleDescriptor obj)
		{
			return base.GetHashCode();
		}

		public bool Equals(IModuleTemplateModuleDescriptor other)
		{
			return base.Equals(other);
		}

		public bool Equals(ModuleTemplateModuleDescriptorBase x, ModuleTemplateModuleDescriptorBase y)
		{
			return Equals(x as IModuleTemplateModuleDescriptor, y as IModuleTemplateModuleDescriptor);
		}

		public int GetHashCode(ModuleTemplateModuleDescriptorBase obj)
		{
			return GetHashCode(obj as IModuleTemplateModuleDescriptor);
		}

		public bool Equals(ModuleTemplateModuleDescriptorBase other)
		{
			return Equals(other as IModuleTemplateModuleDescriptor);
		}
	}
}