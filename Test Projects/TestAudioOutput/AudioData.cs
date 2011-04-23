using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Vixen.Module;

namespace TestAudioOutput {
    [DataContract]
    public class AudioData : IModuleDataModel {
        [DataMember]
        public string FilePath { get; set; }

		public Guid ModuleTypeId { get; set; }

		public Guid ModuleInstanceId { get; set; }

		public IModuleDataSet ModuleDataSet { get; set; }

		public IModuleDataModel Clone() {
			AudioData newInstance = new AudioData();
			newInstance.FilePath = FilePath;
			return newInstance;
		}
	}
}
