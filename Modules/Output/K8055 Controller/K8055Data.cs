using System.IO.Ports;
using System.Runtime.Serialization;
using Vixen.Module;

namespace VixenModules.Output.K8055_Controller
{
	[DataContract]
	public class K8055Data : ModuleDataModelBase
	{
		[DataMember]
		public int StartChannel { get; set; }

		[DataMember]
		public K8055ControlModuleDeviceStarts[] DeviceStarts { get; set; }

		public override IModuleDataModel Clone()
		{
			return MemberwiseClone() as IModuleDataModel;
		}
	}
	
	[DataContract]
	public class K8055ControlModuleDeviceStarts
	{
		public K8055ControlModuleDeviceStarts(int id)
		{
			ID = id;
		}

		[DataMember]
		public int ID { get; set; }
	}
}
