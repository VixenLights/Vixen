using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;

namespace Vixen.Module.Editor {
	class EditorModuleManagement : UnusedModuleManagement<IEditorModuleInstance> {
		public IEditorModuleInstance Get(string filePath) {
			string fileType = System.IO.Path.GetExtension(filePath);

			IEditorModuleDescriptor descriptor = Modules.GetModuleDescriptors<IEditorModuleInstance>().Cast<IEditorModuleDescriptor>().FirstOrDefault(x => x.FileExtensions.Contains(fileType, StringComparer.OrdinalIgnoreCase));
			if(descriptor != null) {
				return Get(descriptor.TypeId);
			}
			return null;
		}
	}
}
