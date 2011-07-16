using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.Module.Media {
	abstract public class MediaModuleDescriptorBase : ModuleDescriptorBase, IMediaModuleDescriptor, IEqualityComparer<IMediaModuleDescriptor> {
		abstract public override string TypeName { get; }

		abstract public override Guid TypeId { get; }

		abstract public override Type ModuleClass { get; }

		abstract public override Type ModuleDataClass { get; }

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
	}
}
