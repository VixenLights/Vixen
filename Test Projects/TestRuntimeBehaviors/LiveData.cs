using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module;
using System.Runtime.Serialization;

namespace TestRuntimeBehaviors {
	[DataContract]
	public class LiveData : IModuleDataModel {
		public LiveData() {
			// Will this suffice for setting a default value while not overwriting
			// the user-set value?
			HeadStart = 50;
		}

		[DataMember]
		public bool Enabled { get; set; }

		[DataMember]
		public int HeadStart { get; set; }

		public Guid ModuleTypeId { get; set; }

		public IModuleDataSet ModuleDataSet { get; set; }

		public Guid ModuleInstanceId { get; set; }

		public IModuleDataModel Clone() {
			LiveData newInstance = new LiveData();
			newInstance.Enabled = this.Enabled;
			return newInstance;
		}
	}
}
