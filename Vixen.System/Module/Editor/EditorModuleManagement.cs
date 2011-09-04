using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;

namespace Vixen.Module.Editor {
	class EditorModuleManagement : GenericModuleManagement<IEditorModuleInstance> {
		public IEditorUserInterface Get(string filePath) {
			string fileType = System.IO.Path.GetExtension(filePath);
			IEditorModuleDescriptor descriptor = Modules.GetDescriptors<IEditorModuleInstance, IEditorModuleDescriptor>().FirstOrDefault(x => x.FileExtensions.Contains(fileType, StringComparer.OrdinalIgnoreCase));
			return _GetEditorUI(descriptor);
		}

		public new IEditorUserInterface Get(Guid id) {
			IEditorModuleDescriptor descriptor = Modules.GetDescriptorById(id) as IEditorModuleDescriptor;
			return _GetEditorUI(descriptor);
		}

		private IEditorUserInterface _GetEditorUI(IEditorModuleDescriptor descriptor) {
			if(descriptor != null) {
				IEditorModuleInstance module = base.Get(descriptor.TypeId);
				IEditorUserInterface moduleUI = Activator.CreateInstance(descriptor.EditorUserInterfaceClass) as IEditorUserInterface;
				moduleUI.OwnerModule = module;
				return moduleUI;
			}
			return null;
		}
	}
}
