using System.Runtime.Serialization;
using Vixen.Data.Value;
using Vixen.Module;

namespace VixenModules.Effect.SetPosition {
	[DataContract]
	public class SetPositionData : ModuleDataModelBase {
		public override IModuleDataModel Clone() {
			return (SetPositionData)MemberwiseClone();
		}

		[DataMember]
		public PositionValue StartPosition { get; set; }

		[DataMember]
		public PositionValue EndPosition { get; set; }
	}
}
