using System.Runtime.Serialization;
using Vixen.Module;

namespace VixenModules.OutputFilter.Color {
	[DataContract]
	public class ColorData : ModuleDataModelBase {
		public ColorData() {
			FilterOrder = new ColorFilter[0];
		}

		public override IModuleDataModel Clone() {
			ColorData newInstance = (ColorData)MemberwiseClone();
			return newInstance;
		}

		[DataMember]
		public ColorFilter[] FilterOrder { get; set; }
	}
}
