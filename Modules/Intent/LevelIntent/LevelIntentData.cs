using System.Runtime.Serialization;
using Vixen.Commands.KnownDataTypes;
using Vixen.Module;

namespace LevelIntent {
	[DataContract]
	public class LevelIntentData : ModuleDataModelBase {
		public override IModuleDataModel Clone() {
			return (LevelIntentData)MemberwiseClone();
		}

		[DataMember]
		public Level StartLevel { get; set; }

		[DataMember]
		public Level EndLevel { get; set; }
	}
}
