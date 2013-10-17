using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Vixen.Module;

namespace VersionControl {
	[DataContract]
	public class Data : ModuleDataModelBase {
		public Data() {
			IsEnabled = false;
		}

		[ DataMember]
		public bool IsEnabled { get; set; }
		
		public override IModuleDataModel Clone() {
			return (Data)MemberwiseClone();
		}
	}
}
