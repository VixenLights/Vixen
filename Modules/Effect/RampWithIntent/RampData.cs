using System.Runtime.Serialization;
using Vixen.Module;

namespace RampWithIntent {
	[DataContract]
	public class RampData : ModuleDataModelBase {
		public override IModuleDataModel Clone() {
			return (RampData)MemberwiseClone();
		}

		[DataMember]
		public float StartLevel { get; set; }

		[DataMember]
		public float EndLevel { get; set; }
	}
}
