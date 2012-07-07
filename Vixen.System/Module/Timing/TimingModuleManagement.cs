using System;

namespace Vixen.Module.Timing {
	class TimingModuleManagement : GenericModuleManagement<ITimingModuleInstance> {
		public ITimingModuleInstance GetDefault() {
			return Get(Guid.Empty);
		}
	}
}
