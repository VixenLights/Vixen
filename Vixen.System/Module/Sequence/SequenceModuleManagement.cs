using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Vixen.Sys;
using Vixen.Common;
using Vixen.Module.ModuleTemplate;

namespace Vixen.Module.Sequence {
	class SequenceModuleManagement : GenericModuleManagement<ISequenceModuleInstance> {
		/// <summary>
		/// Creates a new instance.  Does not load any existing content.
		/// </summary>
		/// <param name="sequenceFileType"></param>
		/// <returns></returns>
		public ISequenceModuleInstance Get(string sequenceFileType) {
			string fileType = Path.GetExtension(sequenceFileType);

			ISequenceModuleDescriptor descriptor = Modules.GetModuleDescriptors<ISequenceModuleInstance>().Cast<ISequenceModuleDescriptor>().FirstOrDefault(x => x.FileExtension.Equals(fileType, StringComparison.OrdinalIgnoreCase));
			if(descriptor != null) {
				ISequenceModuleInstance instance = Get(descriptor.TypeId);

				return instance;
			}
			return null;
		}
	}
}
