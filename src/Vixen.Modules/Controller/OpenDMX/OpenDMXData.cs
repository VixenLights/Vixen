using System.Runtime.Serialization;
using Vixen.Module;

namespace VixenModules.Controller.OpenDMX
{
	[DataContract]
	public class OpenDMXData: ModuleDataModelBase
	{
		[DataMember]
		public Device Device { get; set; }

		public override IModuleDataModel Clone()
		{
			OpenDMXData newInstance = new OpenDMXData() {Device = Device};
			return newInstance;
		}
	}
}
