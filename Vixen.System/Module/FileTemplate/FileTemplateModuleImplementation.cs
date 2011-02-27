using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Common;

namespace Vixen.Module.FileTemplate {
	[ModuleType("FileTemplate")]
	class FileTemplateModuleImplementation : ModuleImplementation<IFileTemplateModuleInstance> {
		public FileTemplateModuleImplementation()
			: base(new FileTemplateModuleType(), new FileTemplateModuleManagement(), new FileTemplateModuleRepository()) {
		}
	}
}
