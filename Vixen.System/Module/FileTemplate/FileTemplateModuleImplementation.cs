using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Common;

namespace Vixen.Module.FileTemplate {
	[TypeOfModule("FileTemplate")]
	class FileTemplateModuleImplementation : ModuleImplementation<IFileTemplateModuleInstance> {
		public FileTemplateModuleImplementation()
			: base(new FileTemplateModuleManagement(), new FileTemplateModuleRepository()) {
		}
	}
}
