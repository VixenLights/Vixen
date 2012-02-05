using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Vixen.Module;

namespace Color {
	[DataContract]
	public class ColorData : ModuleDataModelBase {
		//public ColorData() {
		//    _Initialize();
		//}

		public override IModuleDataModel Clone() {
			ColorData newInstance = new ColorData();
			return newInstance;
		}

		[DataMember]
		public ColorFilter ColorFilter { get; set; }

		//[OnDeserializing]
		//private void OnDeserializing(StreamingContext context) {
		//    _Initialize();
		//}

		//private void _Initialize() {
		//}
	}
}
