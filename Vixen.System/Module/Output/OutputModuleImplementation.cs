using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Common;

namespace Vixen.Module.Output {
	[ModuleType("Output")]
	class OutputModuleImplementation : ModuleImplementation<IOutputModuleInstance> {
		public OutputModuleImplementation()
			: base(new OutputModuleType(), new OutputModuleManagement(), new OutputModuleRepository()) {
		}
	}
}
