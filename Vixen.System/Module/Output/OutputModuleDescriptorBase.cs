using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.Module.Output {
	abstract public class OutputModuleDescriptorBase : ModuleDescriptorBase, IOutputModuleDescriptor, IEqualityComparer<IOutputModuleDescriptor>{
		abstract public override string TypeName { get; }

		abstract public override Guid TypeId { get; }

		abstract public override Type ModuleClass { get; }

		abstract public override Type ModuleDataClass { get; }

		abstract public override string Author { get; }

		abstract public override string Description { get; }

		abstract public override string Version { get; }

		public bool Equals(IOutputModuleDescriptor x, IOutputModuleDescriptor y) {
			return base.Equals(x, y);
		}

		public int GetHashCode(IOutputModuleDescriptor obj) {
			return base.GetHashCode();
		}
	}
}
