using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;

namespace Vixen.Module.Property {
	abstract public class PropertyModuleInstanceBase : ModuleInstanceBase, IPropertyModuleInstance, IEqualityComparer<IPropertyModuleInstance> {
		virtual public ChannelNode Owner { get; set; }

		abstract public void Setup();

		public bool Equals(IPropertyModuleInstance x, IPropertyModuleInstance y) {
			return base.Equals(x, y);
		}

		public int GetHashCode(IPropertyModuleInstance obj) {
			return base.GetHashCode(obj);
		}
	}
}
