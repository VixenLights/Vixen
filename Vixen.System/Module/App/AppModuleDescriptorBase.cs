using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.Module.App {
	abstract public class AppModuleDescriptorBase : ModuleDescriptorBase, IAppModuleDescriptor, IEqualityComparer<IAppModuleDescriptor>, IEquatable<IAppModuleDescriptor>, IEqualityComparer<AppModuleDescriptorBase>, IEquatable<AppModuleDescriptorBase> {
		abstract public override string TypeName { get; }

		abstract public override Guid TypeId { get; }

		abstract public override Type ModuleClass { get; }

		abstract public override Type ModuleDataClass { get; }

		abstract public override string Author { get; }

		abstract public override string Description { get; }

		abstract public override string Version { get; }

		public bool Equals(IAppModuleDescriptor x, IAppModuleDescriptor y) {
			return base.Equals(x, y);
		}

		public int GetHashCode(IAppModuleDescriptor obj) {
			return base.GetHashCode();
		}

		public bool Equals(IAppModuleDescriptor other) {
			return base.Equals(other);
		}

		public bool Equals(AppModuleDescriptorBase x, AppModuleDescriptorBase y) {
			return Equals(x as IAppModuleDescriptor, y as IAppModuleDescriptor);
		}

		public int GetHashCode(AppModuleDescriptorBase obj) {
			return GetHashCode(obj as IAppModuleDescriptor);
		}

		public bool Equals(AppModuleDescriptorBase other) {
			return Equals(other as IAppModuleDescriptor);
		}
	}
}
