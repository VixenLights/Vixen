using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;

namespace Vixen.Module.CommandEditor {
	class CommandEditorModuleType : IModuleLoadNotification {
		public void ModuleLoaded(IModuleDescriptor descriptor) {
			ICommandEditorModuleDescriptor commandEditorDescriptor = descriptor as ICommandEditorModuleDescriptor;
			Server.ModuleRepository.AddCommandEditor(descriptor.TypeId);
		}

		public void ModuleUnloading(IModuleDescriptor descriptor) {
			Server.ModuleRepository.RemoveCommandEditor(descriptor.TypeId);
		}
	}
}
