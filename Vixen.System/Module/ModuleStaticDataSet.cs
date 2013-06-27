using System;
using System.Runtime.Serialization;

namespace Vixen.Module
{
	[DataContract]
	public class ModuleStaticDataSet : ModuleDataSet
	{
		protected override Type _GetDataModelType(IModuleDescriptor descriptor)
		{
			return descriptor.ModuleStaticDataClass;
		}

		protected override IModuleDataModel _GetDataInstance(IModuleInstance module)
		{
			return module.StaticModuleData;
		}
	}
}