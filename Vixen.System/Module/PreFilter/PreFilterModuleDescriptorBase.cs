using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.Module.PreFilter {
	abstract public class PreFilterModuleDescriptorBase : ModuleDescriptorBase, IPreFilterModuleDescriptor, IEqualityComparer<IPreFilterModuleDescriptor>, IEquatable<IPreFilterModuleDescriptor>, IEqualityComparer<PreFilterModuleDescriptorBase>, IEquatable<PreFilterModuleDescriptorBase> {
		abstract public override string TypeName { get; }

		abstract public override Guid TypeId { get; }

		abstract public override Type ModuleClass { get; }

		abstract public override string Author { get; }

		abstract public override string Description { get; }

		abstract public override string Version { get; }

		abstract public string PreFilterName { get; }

		public bool Equals(IPreFilterModuleDescriptor x, IPreFilterModuleDescriptor y) {
			return base.Equals(x, y);
		}

		public int GetHashCode(IPreFilterModuleDescriptor obj) {
			return base.GetHashCode();
		}

		public bool Equals(IPreFilterModuleDescriptor other) {
			return base.Equals(other);
		}

		public bool Equals(PreFilterModuleDescriptorBase x, PreFilterModuleDescriptorBase y) {
			return Equals(x as IPreFilterModuleDescriptor, y as IPreFilterModuleDescriptor);
		}

		public int GetHashCode(PreFilterModuleDescriptorBase obj) {
			return GetHashCode(obj as IPreFilterModuleDescriptor);
		}

		public bool Equals(PreFilterModuleDescriptorBase other) {
			return Equals(other as IPreFilterModuleDescriptor);
		}
	}
}
