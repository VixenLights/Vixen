using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Vixen.Sys;
using Vixen.Common;
using Vixen.Module.FileTemplate;

namespace Vixen.Module.Sequence {
	class SequenceModuleManagement : GenericModuleManagement<ISequenceModuleInstance> {
		/// <summary>
		/// Creates a new instance.  Does not load any existing content.
		/// </summary>
		/// <param name="fileNameOrExtension"></param>
		/// <returns></returns>
		public ISequenceModuleInstance Get(string fileNameOrExtension) {
			string fileType = Path.GetExtension(fileNameOrExtension);

			ISequenceModuleDescriptor descriptor = Modules.GetModuleDescriptors<ISequenceModuleInstance>().Cast<ISequenceModuleDescriptor>().FirstOrDefault(x => x.FileExtension.Equals(fileType, StringComparison.OrdinalIgnoreCase));
			if(descriptor != null) {
				ISequenceModuleInstance instance = Get(descriptor.TypeId);

				//// Apply any template that may exist.
				//FileTemplateModuleManagement manager = Modules.GetModuleManager<IFileTemplateModuleInstance, FileTemplateModuleManagement>();
				//manager.ProjectTemplateInto(descriptor.FileExtension, instance);

				return instance;
			}
			return null;
		}
	}
}
