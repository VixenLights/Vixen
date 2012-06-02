using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys.Attribute;

namespace Vixen.Module.Property {
	[TypeOfModule("Property")]
	class PropertyModuleImplementation : ModuleImplementation<IPropertyModuleInstance> {
		public PropertyModuleImplementation()
			: base(new PropertyModuleManagement(), new PropertyModuleRepository()) {
		}
	}
}
