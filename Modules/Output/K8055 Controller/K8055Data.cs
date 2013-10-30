using System.Collections.Generic;
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
		public K8055ControlModule[] Modules{ get; set; }

		public override IModuleDataModel Clone()
		{
			return MemberwiseClone() as IModuleDataModel;
		}
	}
	
	[DataContract]
	public class K8055ControlModule
	{
		public K8055ControlModule(int id)
		{
			ID = id;
			Enabled = false;
			StartChannel = 1;
			
		}

		[DataMember]
		public bool Enabled { get; set; }

		[DataMember]
		public int StartChannel { get; set; }

		[DataMember]
		public int ID { get; set; }
	}
}
