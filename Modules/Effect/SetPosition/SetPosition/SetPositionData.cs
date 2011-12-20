using System.Runtime.Serialization;
using Vixen.Commands.KnownDataTypes;
using Vixen.Module;

namespace VixenModules.Effect.SetPosition {
	[DataContract]
	public class SetPositionData : ModuleDataModelBase {
		public override IModuleDataModel Clone() {
			return MemberwiseClone() as SetPositionData;
		}

		[DataMember]
		public Position Position { get; set; }
	}
}
