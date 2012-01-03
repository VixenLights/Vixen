using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.Module.Intent {
	abstract public class IntentModuleInstanceBase : ModuleInstanceBase, IIntentModuleInstance, IEqualityComparer<IIntentModuleInstance>, IEquatable<IIntentModuleInstance>, IEqualityComparer<IntentModuleInstanceBase>, IEquatable<IntentModuleInstanceBase> {
		public bool Equals(IIntentModuleInstance x, IIntentModuleInstance y) {
			return base.Equals(x, y);
		}

		public int GetHashCode(IIntentModuleInstance obj) {
			return base.GetHashCode(obj);
		}

		public bool Equals(IIntentModuleInstance other) {
			return base.Equals(other);
		}

		public bool Equals(IntentModuleInstanceBase x, IntentModuleInstanceBase y) {
			return Equals(x as IIntentModuleInstance, y as IIntentModuleInstance);
		}

		public int GetHashCode(IntentModuleInstanceBase obj) {
			return GetHashCode(obj as IIntentModuleInstance);
		}

		public bool Equals(IntentModuleInstanceBase other) {
			return Equals(other as IIntentModuleInstance);
		}
	}
}
