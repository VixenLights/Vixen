using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Vixen.Module;
using Vixen.Commands.KnownDataTypes;

namespace SampleEffect {
	[DataContract]
	public class SampleEffectData : ModuleDataModelBase {
		[DataMember]
		public Level Level { get; set; }

		public override IModuleDataModel Clone() {
			return MemberwiseClone() as IModuleDataModel;
		}
	}
}
