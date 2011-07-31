using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.Module {
	public abstract class ModuleDescriptorBase : IModuleDescriptor, IEqualityComparer<IModuleDescriptor>, IEquatable<IModuleDescriptor>, IEqualityComparer<ModuleDescriptorBase>, IEquatable<ModuleDescriptorBase> {
		abstract public string TypeName { get; }

		abstract public Guid TypeId { get; }

		abstract public Type ModuleClass { get; }

		abstract public Type ModuleDataClass { get; }

		abstract public string Author { get; }

		abstract public string Description { get; }

		abstract public string Version { get; }

		public string FileName { get; set; }

		public System.Reflection.Assembly Assembly { get; set; }

		virtual public Guid[] Dependencies {
			get { return null; }
		}

		public bool Equals(IModuleDescriptor x, IModuleDescriptor y) {
			return x.TypeId == y.TypeId;
		}

		public int GetHashCode(IModuleDescriptor obj) {
			return obj.TypeId.GetHashCode();
		}

		public bool Equals(IModuleDescriptor other) {
			return Equals(this, other);
		}

		public bool Equals(ModuleDescriptorBase x, ModuleDescriptorBase y) {
			return Equals(x as IModuleDescriptor, y as IModuleDescriptor);
		}

		public int GetHashCode(ModuleDescriptorBase obj) {
			return GetHashCode(obj as IModuleDescriptor);
		}

		public bool Equals(ModuleDescriptorBase other) {
			return Equals(other as IModuleDescriptor);
		}

		public override bool Equals(object obj) {
			if(obj is IModuleDescriptor) {
				return Equals(obj as IModuleDescriptor);
			}
			return base.Equals(obj);
		}

		public override int GetHashCode() {
			return GetHashCode(this as IModuleDescriptor);
		}
	}
}
