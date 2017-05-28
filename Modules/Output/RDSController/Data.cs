using System;
using System.Runtime.Serialization;
using Vixen.Module;

namespace VixenModules.Output.CommandController
{
	[DataContract]
	public class Data : ModuleDataModelBase
	{

		[DataMember]
		public string PortName { get; set; }

		[DataMember]
		public int PortNumber { get; set; }

		[DataMember]
		public int ConnectionMode { get; set; }

		[DataMember]
		public bool Slow { get; set; }

		[DataMember]
		public bool BiDirectional { get; set; }

		[DataMember]
		public Hardware HardwareID { get; set; }

		[DataMember]
		public string HttpUrl { get; set; }
		
		[DataMember]
		public bool HideLaunchedWindows { get; set; }

		[DataMember]
		public bool RequireHTTPAuthentication { get; set; }

		[DataMember]
		public string HttpUsername { get; set; }
		
		[DataMember]
		public string HttpPassword { get; set; }
		
		public override IModuleDataModel Clone()
		{
			return this.MemberwiseClone() as IModuleDataModel;
		}


	}
	[Serializable]
	public enum Hardware
	{
		MRDS192=1,
		MRDS1322=2,
		VFMT212R=3,
		HTTP=0
	}
}