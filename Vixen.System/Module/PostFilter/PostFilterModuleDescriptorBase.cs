using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.Module.PostFilter {
	abstract public class PostFilterModuleDescriptorBase : ModuleDescriptorBase, IPostFilterModuleDescriptor, IEqualityComparer<IPostFilterModuleDescriptor>, IEquatable<IPostFilterModuleDescriptor>, IEqualityComparer<PostFilterModuleDescriptorBase>, IEquatable<PostFilterModuleDescriptorBase> {
		abstract public override string TypeName { get; }

		abstract public override Guid TypeId { get; }

		abstract public override Type ModuleClass { get; }

		abstract public override string Author { get; }

		abstract public override string Description { get; }

		abstract public override string Version { get; }

		abstract public string PostFilterName { get; }

		public bool Equals(IPostFilterModuleDescriptor x, IPostFilterModuleDescriptor y) {
			return base.Equals(x, y);
		}

		public int GetHashCode(IPostFilterModuleDescriptor obj) {
			return base.GetHashCode();
		}

		public bool Equals(IPostFilterModuleDescriptor other) {
			return base.Equals(other);
		}

		public bool Equals(PostFilterModuleDescriptorBase x, PostFilterModuleDescriptorBase y) {
			return Equals(x as IPostFilterModuleDescriptor, y as IPostFilterModuleDescriptor);
		}

		public int GetHashCode(PostFilterModuleDescriptorBase obj) {
			return GetHashCode(obj as IPostFilterModuleDescriptor);
		}

		public bool Equals(PostFilterModuleDescriptorBase other) {
			return Equals(other as IPostFilterModuleDescriptor);
		}
	}
}
