using System;
using System.Runtime.Serialization;

namespace Vixen.Module {
	[DataContract]
	public class ModuleStaticDataSet : ModuleDataSet {
		override protected Type _GetDataModelType(IModuleDescriptor descriptor) {
			return descriptor.ModuleStaticDataClass;
		}

		override protected IModuleDataModel _GetDataInstance(IModuleInstance module) {
			return module.StaticModuleData;
		}
	}
}
