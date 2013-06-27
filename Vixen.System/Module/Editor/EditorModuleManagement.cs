using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;

namespace Vixen.Module.Editor
{
	internal class EditorModuleManagement : GenericModuleManagement<IEditorModuleInstance>
	{
		public new IEditorUserInterface Get(Guid id)
		{
			IEditorModuleDescriptor descriptor = Modules.GetDescriptorById(id) as IEditorModuleDescriptor;
			return _GetEditorUI(descriptor);
		}

		public IEditorUserInterface Get(Type sequenceType)
		{
			IEditorModuleDescriptor descriptor =
				Modules.GetDescriptors<IEditorModuleInstance, IEditorModuleDescriptor>().FirstOrDefault(
					x => sequenceType.Equals(x.SequenceType));
			return _GetEditorUI(descriptor);
		}

		private IEditorUserInterface _GetEditorUI(IEditorModuleDescriptor descriptor)
		{
			if (descriptor != null) {
				IEditorModuleInstance module = base.Get(descriptor.TypeId);
				IEditorUserInterface moduleUI =
					Activator.CreateInstance(descriptor.EditorUserInterfaceClass) as IEditorUserInterface;
				moduleUI.OwnerModule = module;
				return moduleUI;
			}
			return null;
		}
	}
}