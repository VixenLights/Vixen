using System;
using System.Runtime.Serialization;
using Vixen.Sys;

namespace Vixen.Module.SequenceType.Surrogate {
	[DataContract(Namespace = "")]
	class ChannelNodeReferenceSurrogate {
		public ChannelNodeReferenceSurrogate(ChannelNode channelNode) {
			NodeId = channelNode.Id;
		}

		[DataMember]
		public Guid NodeId { get; private set; }
	}
}
