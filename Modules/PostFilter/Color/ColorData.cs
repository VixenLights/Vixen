using System.Runtime.Serialization;
using Vixen.Module;

namespace VixenModules.OutputFilter.Color {
	[DataContract]
	public class ColorData : ModuleDataModelBase {
		public override IModuleDataModel Clone() {
			ColorData newInstance = (ColorData)MemberwiseClone();
			return newInstance;
		}

		//[DataMember]
		//public ColorFilter ColorFilter { get; set; }

		//[DataMember]
		//public bool RedSelected { get; set; }

		//[DataMember]
		//public bool GreenSelected { get; set; }

		//[DataMember]
		//public bool BlueSelected { get; set; }

		//[DataMember]
		//public bool YellowSelected { get; set; }

		//[DataMember]
		//public bool WhiteSelected { get; set; }

		[DataMember]
		public ColorFilter[] FilterOrder { get; set; }
	}
}
