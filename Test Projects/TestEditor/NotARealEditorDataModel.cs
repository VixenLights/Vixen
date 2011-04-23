using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Vixen.Module;

namespace TestEditor {
	[DataContract]
	public class NotARealEditorDataModel : IModuleDataModel {
		public Guid ModuleTypeId { get; set; }

		public Guid ModuleInstanceId { get; set; }
		
		public IModuleDataSet ModuleDataSet { get; set; }

		public IModuleDataModel Clone() {
			NotARealEditorDataModel newInstance = new NotARealEditorDataModel();
			newInstance.LastOpened = LastOpened;
			return newInstance;
		}

		[DataMember]
		public DateTime LastOpened { get; set; }
	}
}
