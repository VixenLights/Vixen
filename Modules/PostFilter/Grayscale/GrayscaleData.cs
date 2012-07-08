using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using Vixen.Module;

namespace Grayscale {
	[DataContract]
	public class GrayscaleData : ModuleDataModelBase {
		public override IModuleDataModel Clone() {
			return (GrayscaleData)MemberwiseClone();
		}
	}
}
