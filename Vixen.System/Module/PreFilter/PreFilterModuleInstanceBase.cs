using System;
using System.Collections.Generic;
using Vixen.Commands;

namespace Vixen.Module.PreFilter {
	abstract public class PreFilterModuleInstanceBase : ModuleInstanceBase, IPreFilterModuleInstance, IEqualityComparer<IPreFilterModuleInstance>, IEquatable<IPreFilterModuleInstance>, IEqualityComparer<PreFilterModuleInstanceBase>, IEquatable<PreFilterModuleInstanceBase> {
		abstract public Command Affect(Command command);

		public bool Equals(IPreFilterModuleInstance x, IPreFilterModuleInstance y) {
			return base.Equals(x, y);
		}

		public int GetHashCode(IPreFilterModuleInstance obj) {
			return base.GetHashCode(obj);
		}

		public bool Equals(IPreFilterModuleInstance other) {
			return base.Equals(other);
		}

		public bool Equals(PreFilterModuleInstanceBase x, PreFilterModuleInstanceBase y) {
			return Equals(x as IPreFilterModuleInstance, y as IPreFilterModuleInstance);
		}

		public int GetHashCode(PreFilterModuleInstanceBase obj) {
			return GetHashCode(obj as IPreFilterModuleInstance);
		}

		public bool Equals(PreFilterModuleInstanceBase other) {
			return Equals(other as IPreFilterModuleInstance);
		}
	}
}
