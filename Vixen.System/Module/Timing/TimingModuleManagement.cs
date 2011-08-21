using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;

namespace Vixen.Module.Timing {
	class TimingModuleManagement : GenericModuleManagement<ITimingModuleInstance> {
		// Don't want to implement the interface, just add additional members, so
		// keep subclassing UnusedModuleManagement.
		public ITimingModuleInstance GetDefault() {
			return Get(Guid.Empty);
		}
	}
}
