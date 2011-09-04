using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Vixen.Module;

namespace TestAudioOutput {
	[DataContract]
	public class AudioStaticData : ModuleDataModelBase {
		[DataMember]
		public int SomethingSpecial { get; set; }

		override public IModuleDataModel Clone() {
			return MemberwiseClone() as AudioStaticData;
		}
	}
}
