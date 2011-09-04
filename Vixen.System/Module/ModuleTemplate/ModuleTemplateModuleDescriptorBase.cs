using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.Module.ModuleTemplate {
	abstract public class ModuleTemplateModuleDescriptorBase : ModuleDescriptorBase, IModuleTemplateModuleDescriptor, IEqualityComparer<IModuleTemplateModuleDescriptor>, IEquatable<IModuleTemplateModuleDescriptor>, IEqualityComparer<ModuleTemplateModuleDescriptorBase>, IEquatable<ModuleTemplateModuleDescriptorBase> {
		abstract public override string TypeName { get; }

		abstract public override Guid TypeId { get; }

		abstract public override Type ModuleClass { get; }

		abstract public override string Author { get; }

		abstract public override string Description { get; }

		abstract public override string Version { get; }

		public bool Equals(IModuleTemplateModuleDescriptor x, IModuleTemplateModuleDescriptor y) {
			return base.Equals(x, y);
		}

		public int GetHashCode(IModuleTemplateModuleDescriptor obj) {
			return base.GetHashCode();
		}

		public bool Equals(IModuleTemplateModuleDescriptor other) {
			return base.Equals(other);
		}

		public bool Equals(ModuleTemplateModuleDescriptorBase x, ModuleTemplateModuleDescriptorBase y) {
			return Equals(x as IModuleTemplateModuleDescriptor, y as IModuleTemplateModuleDescriptor);
		}

		public int GetHashCode(ModuleTemplateModuleDescriptorBase obj) {
			return GetHashCode(obj as IModuleTemplateModuleDescriptor);
		}

		public bool Equals(ModuleTemplateModuleDescriptorBase other) {
			return Equals(other as IModuleTemplateModuleDescriptor);
		}
	}
}
