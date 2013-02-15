using System;
using System.Runtime.Serialization;

namespace Vixen.Module.SequenceType.Surrogate {
	[DataContract(Namespace = "")]
	abstract class NodeSurrogate {
		[DataMember]
		public Guid TypeId { get; protected set; }

		[DataMember]
		public Guid InstanceId { get; protected set; }

		[DataMember]
		public TimeSpan StartTime { get; protected set; }

		[DataMember]
		public TimeSpan TimeSpan { get; protected set; }

		[DataMember]
		public ChannelNodeReferenceSurrogate[] TargetNodes { get; protected set; }
	}
}
