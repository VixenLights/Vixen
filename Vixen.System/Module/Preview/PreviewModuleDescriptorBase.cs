using System;
using System.Collections.Generic;

namespace Vixen.Module.Preview {
	abstract public class PreviewModuleDescriptorBase : ModuleDescriptorBase, IPreviewModuleDescriptor, IEqualityComparer<IPreviewModuleDescriptor>, IEquatable<IPreviewModuleDescriptor>, IEqualityComparer<PreviewModuleDescriptorBase>, IEquatable<PreviewModuleDescriptorBase> {
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

		public bool Equals(IPreviewModuleDescriptor x, IPreviewModuleDescriptor y) {
			return base.Equals(x, y);
		}

		public int GetHashCode(IPreviewModuleDescriptor obj) {
			return base.GetHashCode();
		}

		public bool Equals(IPreviewModuleDescriptor other) {
			return base.Equals(other);
		}

		public bool Equals(PreviewModuleDescriptorBase x, PreviewModuleDescriptorBase y) {
			return Equals(x as IPreviewModuleDescriptor, y as IPreviewModuleDescriptor);
		}

		public int GetHashCode(PreviewModuleDescriptorBase obj) {
			return GetHashCode(obj as IPreviewModuleDescriptor);
		}

		public bool Equals(PreviewModuleDescriptorBase other) {
			return Equals(other is IPreviewModuleDescriptor);
		}
	}
}
