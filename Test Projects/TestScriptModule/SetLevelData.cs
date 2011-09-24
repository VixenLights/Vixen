using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Vixen.Sys;
using Vixen.Module;
using CommandStandard.Types;

namespace TestScriptModule {
	[DataContract]
	public class SetLevelData : ModuleDataModelBase {
		[DataMember]
		public Level Level;

		public override IModuleDataModel Clone() {
			return MemberwiseClone() as SetLevelData;
		}
	}
}
