using System.Runtime.Serialization;
using Vixen.Module;

namespace VixenModules.Effect.SetPosition
{
	[DataContract]
	public class SetPositionData : ModuleDataModelBase
	{
		public override IModuleDataModel Clone()
		{
			return (SetPositionData) MemberwiseClone();
		}

		[DataMember]
		public float StartPosition { get; set; }

		[DataMember]
		public float EndPosition { get; set; }
	}
}