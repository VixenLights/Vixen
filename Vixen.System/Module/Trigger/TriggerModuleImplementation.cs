using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Common;

namespace Vixen.Module.Trigger {
	[ModuleType("Trigger")]
	class TriggerModuleImplementation : ModuleImplementation<ITriggerModuleInstance> {
		public TriggerModuleImplementation()
			: base(new TriggerModuleType(), new TriggerModuleManagement(), new TriggerModuleRepository()) {
		}
	}
}
