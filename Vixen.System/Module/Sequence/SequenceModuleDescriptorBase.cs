using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.Module.Sequence {
	abstract public class SequenceModuleDescriptorBase : ModuleDescriptorBase, ISequenceModuleDescriptor, IEqualityComparer<ISequenceModuleDescriptor>, IEquatable<ISequenceModuleDescriptor>, IEqualityComparer<SequenceModuleDescriptorBase>, IEquatable<SequenceModuleDescriptorBase> {
		abstract public override string TypeName { get; }

		abstract public override Guid TypeId { get; }

		abstract public override Type ModuleClass { get; }

		abstract public override string Author { get; }

		abstract public override string Description { get; }

		abstract public override string Version { get; }

		abstract public string FileExtension { get; }

		// Default to true unless overridden in derived class
		virtual public bool CanCreateNew { get { return true; }	}

		public bool Equals(ISequenceModuleDescriptor x, ISequenceModuleDescriptor y) {
			return base.Equals(x, y);
		}

		public int GetHashCode(ISequenceModuleDescriptor obj) {
			return base.GetHashCode(obj);
		}

		public bool Equals(ISequenceModuleDescriptor other) {
			return base.Equals(other);
		}

		public bool Equals(SequenceModuleDescriptorBase x, SequenceModuleDescriptorBase y) {
			return Equals(x as ISequenceModuleDescriptor, y as ISequenceModuleDescriptor);
		}

		public int GetHashCode(SequenceModuleDescriptorBase obj) {
			return GetHashCode(obj as ISequenceModuleDescriptor);
		}

		public bool Equals(SequenceModuleDescriptorBase other) {
			return Equals(other as ISequenceModuleDescriptor);
		}

	}
}
