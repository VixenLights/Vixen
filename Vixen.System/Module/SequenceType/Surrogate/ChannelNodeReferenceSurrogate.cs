using System;
using System.Runtime.Serialization;
using Vixen.Sys;

namespace Vixen.Module.SequenceType.Surrogate
{
	[DataContract(Namespace = "")]
	internal class ChannelNodeReferenceSurrogate
	{
		public ChannelNodeReferenceSurrogate(ElementNode elementNode)
		{
			NodeId = elementNode.Id;
		}

		[DataMember]
		public Guid NodeId { get; private set; }
	}
}