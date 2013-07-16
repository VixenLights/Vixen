using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Vixen.Module;

namespace VixenModules.Effect.RDS {

	[DataContract]
	public class RDSData : ModuleDataModelBase {
	
		[DataMember]
		public string Text { get; set; }

		public override IModuleDataModel Clone() {
			return (IModuleDataModel)this.MemberwiseClone();
		}
	}
}
