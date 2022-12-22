﻿using System.Runtime.Serialization;
using Vixen.Module;

namespace VixenModules.Property.Location {
	[DataContract]
	public class LocationData : ModuleDataModelBase {
		public override IModuleDataModel Clone() {
			return (LocationData)MemberwiseClone();
		}

		[DataMember]
		public int X { get; set; }

		[DataMember]
		public int Y { get; set; }

		[DataMember]
		public int Z { get; set; }
	}
}
