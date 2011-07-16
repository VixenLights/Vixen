using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Common;

namespace Vixen.Module.Timing {
	[TypeOfModule("Timing")]
	class TimingModuleImplementation : ModuleImplementation<ITimingModuleInstance> {
		public TimingModuleImplementation()
			: base(new TimingModuleManagement(), new TimingModuleRepository()) {
		}
	}
}
