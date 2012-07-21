using System;
using System.Collections.Generic;

namespace Vixen.Module.Service {
	abstract public class ServiceModuleInstanceBase : ModuleInstanceBase, IServiceModuleInstance, IEqualityComparer<IServiceModuleInstance>, IEquatable<IServiceModuleInstance>, IEqualityComparer<ServiceModuleInstanceBase>, IEquatable<ServiceModuleInstanceBase> {
		abstract public void Start();

		abstract public void Stop();

		public override IModuleDataModel ModuleData {
			get { return StaticModuleData; }
			set { }
		}

		public override IModuleInstance Clone() {
			// Singleton
			throw new NotSupportedException();
		}

		public bool Equals(IServiceModuleInstance x, IServiceModuleInstance y) {
			return base.Equals(x, y);
		}

		public int GetHashCode(IServiceModuleInstance obj) {
			return base.GetHashCode(obj);
		}

		public bool Equals(IServiceModuleInstance other) {
			return base.Equals(other);
		}

		public bool Equals(ServiceModuleInstanceBase x, ServiceModuleInstanceBase y) {
			return Equals((IServiceModuleInstance)x, (IServiceModuleInstance)y);
		}

		public int GetHashCode(ServiceModuleInstanceBase obj) {
			return GetHashCode((IServiceModuleInstance)obj);
		}

		public bool Equals(ServiceModuleInstanceBase other) {
			return Equals((IServiceModuleInstance)other);
		}
	}
}
