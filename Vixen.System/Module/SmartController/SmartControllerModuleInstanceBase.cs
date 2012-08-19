using System;
using System.Collections.Generic;
using Vixen.Sys;
using Vixen.Sys.Output;

namespace Vixen.Module.SmartController {
	abstract public class SmartControllerModuleInstanceBase : OutputModuleInstanceBase, ISmartControllerModuleInstance, IEqualityComparer<ISmartControllerModuleInstance>, IEquatable<ISmartControllerModuleInstance>, IEqualityComparer<SmartControllerModuleInstanceBase>, IEquatable<SmartControllerModuleInstanceBase> {
		abstract public void UpdateState(IntentChangeCollection[] outputStates);

		#region Equality
		public bool Equals(ISmartControllerModuleInstance x, ISmartControllerModuleInstance y) {
			return base.Equals(x, y);
		}

		public int GetHashCode(ISmartControllerModuleInstance obj) {
			return base.GetHashCode(obj);
		}

		public bool Equals(ISmartControllerModuleInstance other) {
			return base.Equals(other);
		}

		public bool Equals(SmartControllerModuleInstanceBase x, SmartControllerModuleInstanceBase y) {
			return Equals(x as ISmartControllerModuleInstance, y as ISmartControllerModuleInstance);
		}

		public int GetHashCode(SmartControllerModuleInstanceBase obj) {
			return GetHashCode(obj as ISmartControllerModuleInstance);
		}

		public bool Equals(SmartControllerModuleInstanceBase other) {
			return Equals(other as ISmartControllerModuleInstance);
		}
		#endregion
	}
}
