using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;

namespace Vixen.Module.Transform {
	class TransformModuleManagement : GenericModuleManagement<ITransformModuleInstance> {
		override public ITransformModuleInstance Clone(ITransformModuleInstance instance) {
			ITransformModuleInstance newInstance = null;
			if(instance != null) {
				newInstance = Get(instance.Descriptor.TypeId);
				// Assuming to create the new data object as per-instance and not per-type.
				//
				// While a module generally has data, it may be that the instance being
				// cloned has not yet been added to a module dataset, so we're checking for null.
				if(instance.ModuleData != null) {
					newInstance.ModuleData = instance.ModuleData.ModuleDataSet.CloneInstanceData(instance, newInstance);
				}
			}
			return newInstance;
		}
	}
}
