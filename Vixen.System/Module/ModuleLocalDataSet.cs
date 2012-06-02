using System;
using System.Runtime.Serialization;

namespace Vixen.Module {
	[DataContract]
	public class ModuleLocalDataSet : ModuleDataSet {
		override protected Type _GetDataModelType(IModuleDescriptor descriptor) {
			return _GetModuleDataSetType(descriptor);
		}

		protected override IModuleDataModel _GetDataInstance(IModuleInstance module) {
			return module.ModuleData;
		}

		static public IModuleDataModel CreateModuleDataInstance(IModuleInstance module) {
			IModuleDataModel dataModel = null;

			Type moduleDataSetType = _GetModuleDataSetType(module.Descriptor);
			if(moduleDataSetType != null) {
				dataModel = _CreateDataModel(moduleDataSetType);
			}

			return dataModel;
		}

		static private Type _GetModuleDataSetType(IModuleDescriptor descriptor) {
			return descriptor.ModuleDataClass;
		}
	}
}
