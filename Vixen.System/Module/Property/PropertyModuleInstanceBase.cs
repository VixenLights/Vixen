using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;

namespace Vixen.Module.Property {
	abstract public class PropertyModuleInstanceBase : ModuleInstanceBase, IPropertyModuleInstance, IEqualityComparer<IPropertyModuleInstance>, IEquatable<IPropertyModuleInstance>, IEqualityComparer<PropertyModuleInstanceBase>, IEquatable<PropertyModuleInstanceBase> {
		virtual public ChannelNode Owner { get; set; }

		abstract public void Setup();

		public bool Equals(IPropertyModuleInstance x, IPropertyModuleInstance y) {
			return base.Equals(x, y);
		}

		public int GetHashCode(IPropertyModuleInstance obj) {
			return base.GetHashCode(obj);
		}

		public bool Equals(IPropertyModuleInstance other) {
			return base.Equals(other);
		}

		public bool Equals(PropertyModuleInstanceBase x, PropertyModuleInstanceBase y) {
			return Equals(x as IPropertyModuleInstance, y as IPropertyModuleInstance);
		}

		public int GetHashCode(PropertyModuleInstanceBase obj) {
			return GetHashCode(obj as IPropertyModuleInstance);
		}

		public bool Equals(PropertyModuleInstanceBase other) {
			return Equals(other as IPropertyModuleInstance);
		}
	}
}
