using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;
using Vixen.Common;
using Vixen.Module.FileTemplate;

namespace Vixen.Module.Sequence {
	class SequenceModuleManagement : UnusedModuleManagement<ISequenceModuleInstance> {
		public ISequenceModuleInstance Get(string fileNameOrExtension) {
			string fileType = System.IO.Path.GetExtension(fileNameOrExtension);

			ISequenceModuleDescriptor descriptor = Modules.GetModuleDescriptors<ISequenceModuleInstance>().Cast<ISequenceModuleDescriptor>().FirstOrDefault(x => x.FileExtension.Equals(fileType, StringComparison.OrdinalIgnoreCase));
			if(descriptor != null) {
				ISequenceModuleInstance instance = Get(descriptor.TypeId);

				// Apply any template that may exist.
				FileTemplateModuleManagement manager = Server.Internal.GetModuleManager<IFileTemplateModuleInstance, FileTemplateModuleManagement>();
				manager.ProjectTemplateInto(descriptor.FileExtension, instance);

				return instance;
			}
			return null;
		}
	}
}
