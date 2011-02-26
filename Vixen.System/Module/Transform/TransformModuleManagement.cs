using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;

namespace Vixen.Module.Transform {
	class TransformModuleManagement : IModuleManagement<ITransformModuleInstance> {
		public ITransformModuleInstance Get(Guid id) {
			// Need to hook the creation in order to get the pre-cached data into the instance
			ITransformModuleInstance module = Modules.GetById(id) as ITransformModuleInstance;
			if(module != null) {
				// Get the pre-computed command parameters references from the transform
				// module descriptor into the module instance.
				module.CommandsAffected = Modules.GetDescriptorById<ITransformModuleDescriptor>(id).CommandsAffected;
			}
			return module;
		}

		object IModuleManagement.Get(Guid id) {
			return Get(id);
		}

		public ITransformModuleInstance[] GetAll() {
			return Server.ModuleRepository.GetAllTransform();
		}

		object[] IModuleManagement.GetAll() {
			return GetAll();
		}

		public ITransformModuleInstance Clone(ITransformModuleInstance instance) {
			ITransformModuleInstance newInstance = null;
			if(instance != null) {
				newInstance = Get(instance.TypeId);
				// Assuming to create the new data object as per-instance and not per-type.
				newInstance.ModuleData = instance.ModuleData.ModuleDataSet.CloneInstanceData(instance, newInstance);
			}
			return newInstance;
		}

		object IModuleManagement.Clone(object instance) {
			return Clone(instance as ITransformModuleInstance);
		}
	}
}
