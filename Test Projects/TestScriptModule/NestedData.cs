using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Vixen.Module;
using CommandStandard.Types;

namespace TestScriptModule {
	[DataContract]
	public class NestedData : ModuleDataModelBase {
		[DataMember]
		public Level Level;

		public override IModuleDataModel Clone() {
			return MemberwiseClone() as NestedData;
		}
	}
}
