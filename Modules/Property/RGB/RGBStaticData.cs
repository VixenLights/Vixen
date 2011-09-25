using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Vixen.Module;

namespace VixenModules.Property.RGB
{
	[DataContract]
	class RGBStaticData : ModuleDataModelBase
	{
		public override IModuleDataModel Clone()
		{
			throw new NotImplementedException();
		}
	}
}
