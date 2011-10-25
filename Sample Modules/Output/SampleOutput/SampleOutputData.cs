using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Vixen.Module;

namespace SampleOutput {
	[DataContract]
	public class SampleOutputData : ModuleDataModelBase {
		[DataMember]
		public DateTime LastStartDate { get; set; }

		[DataMember]
		public int RunCount { get; set; }

		//***

		public override IModuleDataModel Clone() {
			return (SampleOutputData)this.MemberwiseClone();
		}
	}
}
