using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.Module.Media {
	abstract public class MediaModuleDescriptorBase : ModuleDescriptorBase, IMediaModuleDescriptor, IEqualityComparer<IMediaModuleDescriptor>, IEquatable<IMediaModuleDescriptor>, IEqualityComparer<MediaModuleDescriptorBase>, IEquatable<MediaModuleDescriptorBase> {
		abstract public override string TypeName { get; }

		abstract public override Guid TypeId { get; }

		abstract public override Type ModuleClass { get; }

		abstract public override string Author { get; }

		abstract public override string Description { get; }

		abstract public override string Version { get; }

		abstract public string[] FileExtensions { get; }

		abstract public bool IsTimingSource { get; }

		public bool Equals(IMediaModuleDescriptor x, IMediaModuleDescriptor y) {
			return base.Equals(x, y);
		}

		public int GetHashCode(IMediaModuleDescriptor obj) {
			return base.GetHashCode();
		}

		public bool Equals(IMediaModuleDescriptor other) {
			return base.Equals(other);
		}

		public bool Equals(MediaModuleDescriptorBase x, MediaModuleDescriptorBase y) {
			return Equals(x as IMediaModuleDescriptor, y as IMediaModuleDescriptor);
		}

		public int GetHashCode(MediaModuleDescriptorBase obj) {
			return GetHashCode(obj as IMediaModuleDescriptor);
		}

		public bool Equals(MediaModuleDescriptorBase other) {
			return Equals(other as IMediaModuleDescriptor);
		}
	}
}
