using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;

namespace Vixen.Module.Input {
	[TypeOfModule("Input")]
	class InputModuleImplementation : ModuleImplementation<IInputModuleInstance> {
		public InputModuleImplementation()
			: base(new InputModuleManagement(), new InputModuleRepository()) {
		}
	}
}
