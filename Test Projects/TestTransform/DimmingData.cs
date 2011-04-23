using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Vixen.Module;

namespace TestTransform {
	[DataContract]
	public class DimmingData : IModuleDataModel {
		[DataMember]
		public string DimmingCurveName { get; set; }
		//[DataMember]
		//public int OutputIndex { get; set; }
	
		public Guid ModuleTypeId { get; set; }

		public Guid ModuleInstanceId { get; set; }

		public IModuleDataSet ModuleDataSet { get; set; }

		public IModuleDataModel Clone() {
			DimmingData newInstance = new DimmingData();
			newInstance.DimmingCurveName = DimmingCurveName;
			return newInstance;
		}
	}
}
