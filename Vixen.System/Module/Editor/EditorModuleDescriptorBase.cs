using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.Module.Editor {
	abstract public class EditorModuleDescriptorBase : ModuleDescriptorBase, IEditorModuleDescriptor, IEqualityComparer<IEditorModuleDescriptor> {
		abstract public override string TypeName { get; }

		abstract public override Guid TypeId { get; }

		abstract public override Type ModuleClass { get; }

		abstract public override Type ModuleDataClass { get; }

		abstract public override string Author { get; }

		abstract public override string Description { get; }

		abstract public override string Version { get; }

		abstract public string[] FileExtensions { get; }

		public bool Equals(IEditorModuleDescriptor x, IEditorModuleDescriptor y) {
			return base.Equals(x, y);
		}

		public int GetHashCode(IEditorModuleDescriptor obj) {
			return base.GetHashCode();
		}
	}
}
