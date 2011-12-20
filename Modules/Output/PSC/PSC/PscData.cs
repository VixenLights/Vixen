using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Vixen.Module;

namespace PSC {
	[DataContract]
	public class PscData : ModuleDataModelBase {
		public PscData() {
			// Make sure we're valid from the start so that the baud rate can be controlled.
			PortName = SerialPort.GetPortNames().FirstOrDefault();
			BaudRate = 38400;
			Parity = Parity.None;
			DataBits = 8;
			StopBits = StopBits.One;
		}

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
		//[DataMember]
		//public int WriteTimeout { get; set; }

		override public IModuleDataModel Clone() {
			return this.MemberwiseClone() as IModuleDataModel;
		}

		public bool IsValid {
			get {
				return
					PortName != null &&
					BaudRate != 0 &&
					DataBits != 0 &&
					StopBits != 0;
			}
		}
	}
}
