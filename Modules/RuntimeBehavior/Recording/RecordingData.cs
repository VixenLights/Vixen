using System.Runtime.Serialization;
using Vixen.Module;

namespace Recording
{
	[DataContract]
	public class RecordingData : ModuleDataModelBase
	{
		[DataMember]
		public bool Enabled { get; set; }

		public override IModuleDataModel Clone()
		{
			IModuleDataModel newInstance = MemberwiseClone() as IModuleDataModel;
			return newInstance;
		}
	}
}