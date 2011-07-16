using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.Module.FileTemplate {
	abstract public class FileTemplateModuleDescriptorBase : ModuleDescriptorBase, IFileTemplateModuleDescriptor, IEqualityComparer<IFileTemplateModuleDescriptor> {
		abstract public override string TypeName { get; }

		abstract public override Guid TypeId { get; }

		abstract public override Type ModuleClass { get; }

		abstract public override Type ModuleDataClass { get; }

		abstract public override string Author { get; }

		abstract public override string Description { get; }

		abstract public override string Version { get; }

		abstract public string FileType { get; }

		public bool Equals(IFileTemplateModuleDescriptor x, IFileTemplateModuleDescriptor y) {
			return base.Equals(x, y);
		}

		public int GetHashCode(IFileTemplateModuleDescriptor obj) {
			return base.GetHashCode();
		}
	}
}
