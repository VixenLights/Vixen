﻿using System.IO.Ports;
using System.Runtime.Serialization;
using Vixen.Module;

namespace VixenModules.Output.Renard
{
	[DataContract]
	public class Data : ModuleDataModelBase
	{
		[DataMember]
		public string PortName { get; set; }

		[DataMember]
		public int BaudRate { get; set; }

		[DataMember]
		public Parity Parity { get; set; }

		[DataMember]
		public int DataBits { get; set; }

		[DataMember]
		public StopBits StopBits { get; set; }

		[DataMember]
		public int ProtocolVersion { get; set; }

		[DataMember]
		public int WriteTimeout { get; set; }

		public override IModuleDataModel Clone()
		{
			return this.MemberwiseClone() as IModuleDataModel;
		}

		public bool IsValid
		{
			get
			{
				return
					PortName != null &&
					BaudRate != 0 &&
					DataBits != 0 &&
					StopBits != 0;
			}
		}
	}
}