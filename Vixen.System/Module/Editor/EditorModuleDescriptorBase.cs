using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.Module.Editor
{
	public abstract class EditorModuleDescriptorBase : ModuleDescriptorBase, IEditorModuleDescriptor,
	                                                   IEqualityComparer<IEditorModuleDescriptor>,
	                                                   IEquatable<IEditorModuleDescriptor>,
	                                                   IEqualityComparer<EditorModuleDescriptorBase>,
	                                                   IEquatable<EditorModuleDescriptorBase>
	{
		public abstract override string TypeName { get; }

		public abstract override Guid TypeId { get; }

		public abstract override Type ModuleClass { get; }

		public abstract override string Author { get; }

		public abstract override string Description { get; }

		public abstract override string Version { get; }

		public abstract Type EditorUserInterfaceClass { get; }

		public abstract Type SequenceType { get; }

		public bool Equals(IEditorModuleDescriptor x, IEditorModuleDescriptor y)
		{
			return base.Equals(x, y);
		}

		public int GetHashCode(IEditorModuleDescriptor obj)
		{
			return base.GetHashCode();
		}

		public bool Equals(IEditorModuleDescriptor other)
		{
			return base.Equals(other);
		}

		public bool Equals(EditorModuleDescriptorBase x, EditorModuleDescriptorBase y)
		{
			return Equals(x as IEditorModuleDescriptor, y as IEditorModuleDescriptor);
		}

		public int GetHashCode(EditorModuleDescriptorBase obj)
		{
			return GetHashCode(obj as IEditorModuleDescriptor);
		}

		public bool Equals(EditorModuleDescriptorBase other)
		{
			return Equals(other as IEditorModuleDescriptor);
		}
	}
}