using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Vixen.Module;

namespace TestTemplate {
	[DataContract]
	public class InstanceReference {
		public InstanceReference(IModuleInstance instance) {
			TypeId = instance.TypeId;
			InstanceId = instance.InstanceId;
		}

		private InstanceReference(Guid typeId, Guid instanceId) {
			TypeId = typeId;
			InstanceId = instanceId;
		}

		[DataMember]
		public Guid TypeId;
		[DataMember]
		public Guid InstanceId;

		public InstanceReference Clone() {
			return new InstanceReference(TypeId, InstanceId);
		}
	}
}
