using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.Module {
	public abstract class ModuleInstanceBase : IModuleInstance, IEqualityComparer<IModuleInstance> {
		public Guid InstanceId { get; set; }

		virtual public IModuleDataModel ModuleData { get; set; }

		virtual public IModuleDescriptor Descriptor { get; set; }

		virtual public void Dispose() { }

		public bool Equals(IModuleInstance x, IModuleInstance y) {
			return x.InstanceId == y.InstanceId;
		}

		public int GetHashCode(IModuleInstance obj) {
			return obj.InstanceId.GetHashCode();
		}
	}
}
