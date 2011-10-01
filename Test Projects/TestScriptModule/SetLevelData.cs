using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Vixen.Sys;
using Vixen.Module;
using Vixen.Commands.KnownDataTypes;

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
