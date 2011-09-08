using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;

namespace Vixen.Module.Media {
	class MediaModuleManagement : GenericModuleManagement<IMediaModuleInstance> {
		public IMediaModuleInstance Get(string filePath) {
			string fileType = System.IO.Path.GetExtension(filePath);

			IMediaModuleDescriptor descriptor = Modules.GetDescriptors<IMediaModuleInstance, IMediaModuleDescriptor>().FirstOrDefault(x => x.FileExtensions.Contains(fileType, StringComparer.OrdinalIgnoreCase));
			if(descriptor != null) {
				return Get(descriptor.TypeId);
			}
			return null;
		}
	}
}
