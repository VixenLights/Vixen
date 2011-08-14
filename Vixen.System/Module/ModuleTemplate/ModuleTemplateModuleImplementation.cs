using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Common;

namespace Vixen.Module.ModuleTemplate {
	[TypeOfModule("ModuleTemplate")]
	class ModuleTemplateModuleImplementation : ModuleImplementation<IModuleTemplateModuleInstance> {
		public ModuleTemplateModuleImplementation()
			: base(new ModuleTemplateModuleManagement(), new ModuleTemplateModuleRepository()) {
		}
	}
}
