using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Vixen.Module;

namespace _595 {
	[DataContract]
	public class Data : ModuleDataModelBase {
		[DataMember]
		public ushort Port { get; set; }

		public override IModuleDataModel Clone() {
			Data newInstance = new Data() { Port = Port };
			return newInstance;
		}
	}
}
