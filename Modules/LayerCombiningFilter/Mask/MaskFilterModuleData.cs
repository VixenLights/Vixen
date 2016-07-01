using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Vixen.Module;

namespace VixenModules.LayerMixingFilter.Mask
{
	public class MaskFilterModuleData: ModuleDataModelBase
	{
		public override IModuleDataModel Clone()
		{
			return (IModuleDataModel)MemberwiseClone();
		}

		[DataMember]
		public int TestData { get; set; }
	}
}
