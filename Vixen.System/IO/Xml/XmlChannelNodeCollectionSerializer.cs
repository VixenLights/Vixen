using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Vixen.Sys;

namespace Vixen.IO.Xml {
	class XmlChannelNodeCollectionSerializer : IXmlSerializer<IEnumerable<ChannelNode>> {
		private const string ELEMENT_NODES = "Nodes";

		public XElement WriteObject(IEnumerable<ChannelNode> value) {
			XmlChannelNodeSerializer channelNodeSerializer = new XmlChannelNodeSerializer();
			IEnumerable<XElement> elements = value.Select(channelNodeSerializer.WriteObject);
			return new XElement(ELEMENT_NODES, elements);
		}

		public IEnumerable<ChannelNode> ReadObject(XElement element) {
			// Any references to non-existent channels will be pruned by this operation.
			List<ChannelNode> channelNodes = new List<ChannelNode>();

			XElement parentNode = element.Element(ELEMENT_NODES);
			if(parentNode != null) {
				XmlChannelNodeSerializer channelNodeSerializer = new XmlChannelNodeSerializer();
				IEnumerable<ChannelNode> childNodes = parentNode.Elements().Select(channelNodeSerializer.ReadObject).NotNull();
				channelNodes.AddRange(childNodes);
			}

			return channelNodes;
		}
	}
}
