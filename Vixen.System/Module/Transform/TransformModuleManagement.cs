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
				if(instance.ModuleData != null) {
					if(instance.ModuleData.ModuleDataSet != null) {
						newInstance.ModuleData = instance.ModuleData.ModuleDataSet.CloneInstanceData(instance, newInstance);
					} else {
						// No dataset, clone it as orphaned data (data will not be cloned
						// in a dataset).
						newInstance.ModuleData = instance.ModuleData.Clone();
					}
				}
			}
			return newInstance;
		}
	}
}
