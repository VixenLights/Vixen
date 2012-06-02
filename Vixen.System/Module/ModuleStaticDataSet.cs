using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Vixen.Module {
	[DataContract]
	public class ModuleStaticDataSet : ModuleDataSet {
		override protected Type _GetDataModelType(IModuleDescriptor descriptor) {
			return _GetModuleDataSetType(descriptor);
		}

		protected override IModuleDataModel _GetDataInstance(IModuleInstance module) {
			return module.StaticModuleData;
		}

		//static public IModuleDataModel CreateModuleDataInstance(IModuleInstance module) {
		//    return _CreateModuleDataInstance(_GetModuleDataSetType(module.Descriptor), module.Descriptor.TypeId, module.InstanceId);
		//}

		static private Type _GetModuleDataSetType(IModuleDescriptor descriptor) {
			return descriptor.ModuleStaticDataClass;
		}
	}
}
