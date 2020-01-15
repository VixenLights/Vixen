using System;
using System.Collections.Generic;
using Vixen.Module;
using System.Runtime.Serialization;
using System.IO.Ports;


namespace VixenModules.Output.FGDimmer
{
	[DataContract]
	public class FGDimmerData : ModuleDataModelBase
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
		public int StartChannel { get; set; }

		[DataMember]
		public int EndChannel { get; set; }

		[DataMember]
		public FGDimmerControlModule[] Modules { get; set; }

		[DataMember]
		public bool HoldPortOpen { get; set; }

		[DataMember]
		public bool AcOperation { get; set; }

		public override IModuleDataModel Clone()
		{
			return MemberwiseClone() as IModuleDataModel;
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