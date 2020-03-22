using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Vixen.Module;
using System.IO.Ports;
using System.Net;

namespace VixenModules.Output.ElexolEtherIO
{

	[DataContract]
	public class ElexolEtherIOData : ModuleDataModelBase
	{

		override public IModuleDataModel Clone()
		{
			return MemberwiseClone() as IModuleDataModel;
		}

		#region IP Setup
		[DataMember]
		public IPAddress Address { get; set; }

		[DataMember]
		public int MinimumIntensity { get; set; }

		[DataMember]
		public int Port { get; set; }

		#endregion
	}
}
