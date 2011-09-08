using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;

namespace Vixen.Module.MediaRenderer {
	class MediaRendererModuleManagement : GenericModuleManagement<IMediaRendererModuleInstance> {
		public IMediaRendererModuleInstance Get(string filePath) {
			string fileType = System.IO.Path.GetExtension(filePath);

			IMediaRendererModuleDescriptor descriptor = Modules.GetDescriptors<IMediaRendererModuleInstance, IMediaRendererModuleDescriptor>().FirstOrDefault(x => x.FileExtensions.Contains(fileType, StringComparer.OrdinalIgnoreCase));
			if(descriptor != null) {
				return Get(descriptor.TypeId);
			}
			return null;
		}
	}
}
