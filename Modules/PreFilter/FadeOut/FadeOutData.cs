using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Vixen.Module;

namespace FadeOut {
	[DataContract]
	public class FadeOutData : ModuleDataModelBase {
		public override IModuleDataModel Clone() {
			return (FadeOutData)MemberwiseClone();
		}
	}
}
