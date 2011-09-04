using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Runtime.Serialization;
using Vixen.Module;

namespace Renard {
	[DataContract]
	public class Data : ModuleDataModelBase {
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

		override public IModuleDataModel Clone() {
			return this.MemberwiseClone() as IModuleDataModel;
		}
	}
}
