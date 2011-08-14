using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.Module.ModuleTemplate {
	public interface IModuleTemplate {
		void Project(IModuleInstance target);
		void Setup();
	}
}
