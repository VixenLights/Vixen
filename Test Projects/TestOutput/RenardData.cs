using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Vixen.Module;

namespace TestOutput {
	[DataContract]
	public class RenardData : ModuleDataModelBase {
		[DataMember]
		public int RunCount;

		public override IModuleDataModel Clone() {
			return MemberwiseClone() as IModuleDataModel;
		}
	}
}
