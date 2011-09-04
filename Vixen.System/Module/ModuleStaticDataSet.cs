using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Vixen.Module {
	[DataContract]
	public class ModuleStaticDataSet : ModuleDataSet {
		override protected Type _GetDataSetType(IModuleDescriptor descriptor) {
			return descriptor.ModuleStaticDataClass;
		}
	}
}
