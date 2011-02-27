using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Common;

namespace Vixen.Module.Input {
	[ModuleType("Input")]
	class InputModuleImplementation : ModuleImplementation<IInputModuleInstance> {
		public InputModuleImplementation()
			: base(new InputType(), new InputModuleManagement(), new InputModuleRepository()) {
		}
	}
}
