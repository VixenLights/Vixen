using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.Module {
	public abstract class ModuleInstanceBase : IModuleInstance, IEqualityComparer<IModuleInstance>, IEquatable<IModuleInstance>, IEqualityComparer<ModuleInstanceBase>, IEquatable<ModuleInstanceBase> {
		public Guid InstanceId { get; set; }

		virtual public Type ModuleDataClass { get { return null; } }

		virtual public IModuleDataModel ModuleData { get; set; }

		virtual public IModuleDataModel StaticModuleData { get; set; }

		virtual public IModuleDescriptor Descriptor { get; set; }

		virtual public void Dispose() { }

		public bool Equals(IModuleInstance x, IModuleInstance y) {
			return x.InstanceId == y.InstanceId;
		}

		public int GetHashCode(IModuleInstance obj) {
			return obj.InstanceId.GetHashCode();
		}

		public bool Equals(IModuleInstance other) {
			return Equals(this, other);
		}

		public bool Equals(ModuleInstanceBase x, ModuleInstanceBase y) {
			return Equals(x as IModuleInstance, y as IModuleInstance);
		}

		public int GetHashCode(ModuleInstanceBase obj) {
			return GetHashCode(obj as IModuleInstance);
		}

		public bool Equals(ModuleInstanceBase other) {
			return Equals(other as IModuleInstance);
		}

		public override bool Equals(object obj) {
			if(obj is IModuleInstance) {
				return Equals(obj as IModuleInstance);
			}
			return base.Equals(obj);
		}

		public override int GetHashCode() {
			return GetHashCode(this as IModuleInstance);
		}
	}
}
