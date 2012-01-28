using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;

namespace Vixen.Module.PreFilter {
	[TypeOfModule("PreFilter")]
	class PreFilterModuleImplementation : ModuleImplementation<IPreFilterModuleInstance> {
		public PreFilterModuleImplementation()
			: base(new PreFilterModuleManagement(), new PreFilterModuleRepository()) {
		}
	}
}
