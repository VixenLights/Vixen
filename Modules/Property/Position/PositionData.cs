using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module;
using System.Runtime.Serialization;

namespace VixenModules.Property.Position {
	[DataContract]
	public class PositionData : ModuleDataModelBase {
		public PositionData() {
			ChildrenPositions = new PositionMap();
		}

		public override IModuleDataModel Clone() {
			PositionData newInstance = new PositionData();
			newInstance.ChildrenPositions.AddRange(this.ChildrenPositions);
			return newInstance;
		}

		[DataMember]
		public PositionMap ChildrenPositions { get; set; }
	}
}
