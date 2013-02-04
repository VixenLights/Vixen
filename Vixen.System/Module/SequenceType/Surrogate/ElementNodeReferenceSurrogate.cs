using System;
using System.Runtime.Serialization;
using Vixen.Sys;

namespace Vixen.Module.SequenceType.Surrogate {
	[DataContract(Namespace = "")]
	class ElementNodeReferenceSurrogate {
		public ElementNodeReferenceSurrogate(ElementNode elementNode) {
			NodeId = elementNode.Id;
		}

		[DataMember]
		public Guid NodeId { get; private set; }
	}
}
