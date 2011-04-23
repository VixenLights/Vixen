using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Common;
using Vixen.Sys;

namespace Vixen.Module.Media {
	class MediaModuleManagement : UnusedModuleManagement<IMediaModuleInstance> {
		public IMediaModuleInstance Get(string filePath) {
			string fileType = System.IO.Path.GetExtension(filePath);

			IMediaModuleDescriptor descriptor = Modules.GetModuleDescriptors<IMediaModuleInstance>().Cast<IMediaModuleDescriptor>().FirstOrDefault(x => x.FileExtensions.Contains(fileType, StringComparer.OrdinalIgnoreCase));
			if(descriptor != null) {
				return Get(descriptor.TypeId);
			}
			return null;
		}
	}
}
