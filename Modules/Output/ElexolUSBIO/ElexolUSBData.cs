using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Vixen.Module;
using System.IO.Ports;

namespace VixenModules.Output.ElexolUSBIO
{

	[DataContract]
	public class ElexolUSBData : ModuleDataModelBase
	{
		// This section would be for a typical serial setup that needs to be stored in the modulestore.xml
		//file for the controller.
		#region Serial port setup
		[DataMember]
		public string PortName  { get; set; }
		[DataMember]
		public int BaudRate { get; set; }
		[DataMember]
		public Parity Parity { get; set; }
		[DataMember]
		public int DataBits { get; set; }
		[DataMember]
		public StopBits StopBits { get; set; }
		[DataMember]
		public int MinimumIntensity {get;set;}

		override public IModuleDataModel Clone()
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
		#endregion
	}
}
