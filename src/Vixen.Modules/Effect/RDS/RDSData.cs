﻿using System.Runtime.Serialization;
using Vixen.Module;

namespace VixenModules.Effect.RDS {

	[DataContract]
	public class RDSData : ModuleDataModelBase {
	
		[DataMember]
		public string Title { get; set; }
		[DataMember]
		public string Artist { get; set; }

		public override IModuleDataModel Clone() {
			return (IModuleDataModel)this.MemberwiseClone();
		}
	}
}
