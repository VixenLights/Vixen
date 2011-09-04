using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.Module.Output {
	abstract public class OutputModuleDescriptorBase : ModuleDescriptorBase, IOutputModuleDescriptor, IEqualityComparer<IOutputModuleDescriptor>, IEquatable<IOutputModuleDescriptor>, IEqualityComparer<OutputModuleDescriptorBase>, IEquatable<OutputModuleDescriptorBase> {
		private const int DEFAULT_UPDATE_INTERVAL = 20;

		abstract public override string TypeName { get; }

		abstract public override Guid TypeId { get; }

		abstract public override Type ModuleClass { get; }

		abstract public override string Author { get; }

		abstract public override string Description { get; }

		abstract public override string Version { get; }

		virtual public int UpdateInterval {
			get { return DEFAULT_UPDATE_INTERVAL; }
		}

		public bool Equals(IOutputModuleDescriptor x, IOutputModuleDescriptor y) {
			return base.Equals(x, y);
		}

		public int GetHashCode(IOutputModuleDescriptor obj) {
			return base.GetHashCode();
		}

		public bool Equals(IOutputModuleDescriptor other) {
			return base.Equals(other);
		}

		public bool Equals(OutputModuleDescriptorBase x, OutputModuleDescriptorBase y) {
			return Equals(x as IOutputModuleDescriptor, y as IOutputModuleDescriptor);
		}

		public int GetHashCode(OutputModuleDescriptorBase obj) {
			return GetHashCode(obj as IOutputModuleDescriptor);
		}

		public bool Equals(OutputModuleDescriptorBase other) {
			return Equals(other is IOutputModuleDescriptor);
		}
	}
}
