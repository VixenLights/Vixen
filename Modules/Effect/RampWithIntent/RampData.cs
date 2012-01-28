using System.Runtime.Serialization;
using Vixen.Commands.KnownDataTypes;
using Vixen.Module;

namespace RampWithIntent {
	[DataContract]
	public class RampData : ModuleDataModelBase {
		public override IModuleDataModel Clone() {
			return (RampData)MemberwiseClone();
		}

		[DataMember]
		public Level StartLevel { get; set; }

		[DataMember]
		public Level EndLevel { get; set; }
	}
}
