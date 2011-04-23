using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module;
using System.Runtime.Serialization;

namespace TestRuntimeBehaviors {
	[DataContract]
	public class LiveTimedData : IModuleDataModel {
		[DataMember]
		public bool Enabled { get; set; }

		public Guid ModuleTypeId { get; set; }

		public IModuleDataSet ModuleDataSet { get; set; }

		public Guid ModuleInstanceId { get; set; }

		public IModuleDataModel Clone() {
			LiveTimedData newInstance = new LiveTimedData();
			newInstance.Enabled = this.Enabled;
			return newInstance;
		}
	}
}
