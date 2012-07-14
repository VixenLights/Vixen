using System.Runtime.Serialization;
using Vixen.Module;

namespace VixenModules.OutputFilter.Color {
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
