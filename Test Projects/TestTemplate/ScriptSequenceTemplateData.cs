using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Xml.Linq;
using Vixen.Module;
using Vixen.Module.RuntimeBehavior;

namespace TestTemplate {
	[DataContract]
	public class ScriptSequenceTemplateData : IModuleDataModel {
		public ScriptSequenceTemplateData() {
			Behaviors = new List<Guid>();
			// Default values.
			Length = Vixen.Sys.ScriptSequence.Forever;
		}

		[DataMember]
		public List<Guid> Behaviors { get; set; }

		[DataMember]
		public TimeSpan Length { get; set; }

		public Guid ModuleTypeId { get; set; }

		public IModuleDataSet ModuleDataSet { get; set; }

		public Guid ModuleInstanceId { get; set; }

		public IModuleDataModel Clone() {
			ScriptSequenceTemplateData newInstance = new ScriptSequenceTemplateData();
			newInstance.Behaviors = new List<Guid>(Behaviors);
			return newInstance;
		}
	}
}
