using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Vixen.Module;

namespace Color {
	[DataContract]
	public class ColorData : ModuleDataModelBase {
		public override IModuleDataModel Clone() {
			ColorData newInstance = (ColorData)MemberwiseClone();
			return newInstance;
		}

		[DataMember]
		public ColorFilter ColorFilter { get; set; }
	}
}
