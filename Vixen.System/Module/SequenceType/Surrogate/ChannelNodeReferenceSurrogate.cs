using System;
using System.Runtime.Serialization;
using Vixen.Sys;

namespace Vixen.Module.SequenceType.Surrogate
{
	[DataContract(Namespace = "")]
	internal class ChannelNodeReferenceSurrogate
	{
		public ChannelNodeReferenceSurrogate(IElementNode elementNode)
		{
			NodeId = elementNode.Id;
			Name = elementNode.Name;
		}

		[DataMember]
		public Guid NodeId { get; private set; }

		[DataMember]
		public string Name { get; private set; }
	}
}