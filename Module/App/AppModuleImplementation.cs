using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Common;

namespace Vixen.Module.App {
	[ModuleType("App")]
	class AppModuleImplementation : ModuleImplementation<IAppModuleInstance> {
		public AppModuleImplementation()
			: base(new AppModuleType(), new AppModuleManagement(), new AppModuleRepository()) {
		}
	}
}
