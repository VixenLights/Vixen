using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.Module.MediaRenderer {
	abstract public class MediaRendererModuleDescriptorBase : ModuleDescriptorBase, IMediaRendererModuleDescriptor, IEqualityComparer<IMediaRendererModuleDescriptor>, IEquatable<IMediaRendererModuleDescriptor>, IEqualityComparer<MediaRendererModuleDescriptorBase>, IEquatable<MediaRendererModuleDescriptorBase> {
		abstract public override string TypeName { get; }

		abstract public override Guid TypeId { get; }

		abstract public override Type ModuleClass { get; }

		abstract public override string Author { get; }

		abstract public override string Description { get; }

		abstract public override string Version { get; }

		abstract public string[] FileExtensions { get; }

		public bool Equals(IMediaRendererModuleDescriptor x, IMediaRendererModuleDescriptor y) {
			return base.Equals(x, y);
		}

		public int GetHashCode(IMediaRendererModuleDescriptor obj) {
			return base.GetHashCode();
		}

		public bool Equals(IMediaRendererModuleDescriptor other) {
			return base.Equals(other);
		}

		public bool Equals(MediaRendererModuleDescriptorBase x, MediaRendererModuleDescriptorBase y) {
			return Equals(x as IMediaRendererModuleDescriptor, y as IMediaRendererModuleDescriptor);
		}

		public int GetHashCode(MediaRendererModuleDescriptorBase obj) {
			return GetHashCode(obj as IMediaRendererModuleDescriptor);
		}

		public bool Equals(MediaRendererModuleDescriptorBase other) {
			return Equals(other as IMediaRendererModuleDescriptor);
		}
	}
}
