using System.Runtime.Serialization;
using Vixen.Module;

namespace VixenModules.Property.Orientation {
	[DataContract]
	public class OrientationData : ModuleDataModelBase {
		public override IModuleDataModel Clone() {
			return (OrientationData)MemberwiseClone();
		}

		[DataMember]
		public Orientation Orientation { get; set; }

		
	}
}
