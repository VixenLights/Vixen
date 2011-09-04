using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.Module.Editor {
	abstract public class EditorModuleDescriptorBase : ModuleDescriptorBase, IEditorModuleDescriptor, IEqualityComparer<IEditorModuleDescriptor>, IEquatable<IEditorModuleDescriptor>, IEqualityComparer<EditorModuleDescriptorBase>, IEquatable<EditorModuleDescriptorBase> {
		abstract public override string TypeName { get; }

		abstract public override Guid TypeId { get; }

		abstract public override Type ModuleClass { get; }

		abstract public override Type ModuleDataClass { get; }

		abstract public override string Author { get; }

		abstract public override string Description { get; }

		abstract public override string Version { get; }

		abstract public string[] FileExtensions { get; }

		abstract public Type EditorUserInterfaceClass { get; }

		public bool Equals(IEditorModuleDescriptor x, IEditorModuleDescriptor y) {
			return base.Equals(x, y);
		}

		public int GetHashCode(IEditorModuleDescriptor obj) {
			return base.GetHashCode();
		}

		public bool Equals(IEditorModuleDescriptor other) {
			return base.Equals(other);
		}

		public bool Equals(EditorModuleDescriptorBase x, EditorModuleDescriptorBase y) {
			return Equals(x as IEditorModuleDescriptor, y as IEditorModuleDescriptor);
		}

		public int GetHashCode(EditorModuleDescriptorBase obj) {
			return GetHashCode(obj as IEditorModuleDescriptor);
		}

		public bool Equals(EditorModuleDescriptorBase other) {
			return Equals(other as IEditorModuleDescriptor);
		}
	}
}
