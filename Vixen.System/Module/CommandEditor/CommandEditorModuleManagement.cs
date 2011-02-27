using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;
using Vixen.Module.CommandSpec;

namespace Vixen.Module.CommandEditor {
	class CommandEditorModuleManagement : UnusedModuleManagement<ICommandEditorModuleInstance> {
		public ICommandEditorControl GetCommandEditor(Guid commandSpecId) {
			ICommandEditorModuleInstance instance = null;

			// Need the type-specific repository reference, doing more than basic
			// repository operations.
			CommandEditorModuleRepository repository = Server.Internal.GetModuleRepository<ICommandEditorModuleInstance, CommandEditorModuleRepository>();
			// Get the command spec descriptor.
			ICommandSpecModuleDescriptor descriptor = Modules.GetDescriptorById<ICommandSpecModuleDescriptor>(commandSpecId);
			// Look up by command handler id first, then command signature.
			instance = repository.GetByCommandSpecId(descriptor.TypeId) ?? repository.Get(descriptor.Parameters);
			if(instance != null) {
				return instance.CreateEditorControl();
			}
			return null;
		}
	}
}
