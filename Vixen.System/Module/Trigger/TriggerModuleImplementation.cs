using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;

namespace Vixen.Module.Trigger {
	[TypeOfModule("Trigger")]
	class TriggerModuleImplementation : ModuleImplementation<ITriggerModuleInstance> {
		public TriggerModuleImplementation()
			: base(new TriggerModuleManagement(), new TriggerModuleRepository()) {
		}
	}
}
