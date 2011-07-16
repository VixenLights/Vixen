using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.Module.Sequence {
	abstract public class SequenceModuleDescriptorBase : ModuleDescriptorBase, ISequenceModuleDescriptor, IEqualityComparer<ISequenceModuleDescriptor> {
		abstract public override string TypeName { get; }

		abstract public override Guid TypeId { get; }

		abstract public override Type ModuleClass { get; }

		abstract public override Type ModuleDataClass { get; }

		abstract public override string Author { get; }

		abstract public override string Description { get; }

		abstract public override string Version { get; }

		abstract public string FileExtension { get; }

		public bool Equals(ISequenceModuleDescriptor x, ISequenceModuleDescriptor y) {
			return base.Equals(x, y);
		}

		public int GetHashCode(ISequenceModuleDescriptor obj) {
			return base.GetHashCode(obj);
		}
	}
}
