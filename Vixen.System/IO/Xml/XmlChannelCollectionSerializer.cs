using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Vixen.Sys;

namespace Vixen.IO.Xml {
	class XmlChannelCollectionSerializer : IXmlSerializer<IEnumerable<Channel>> {
		private const string ELEMENT_CHANNELS = "Channels";

		public XElement WriteObject(IEnumerable<Channel> value) {
			XmlChannelSerializer channelSerializer = new XmlChannelSerializer();
			IEnumerable<XElement> elements = value.Select(channelSerializer.WriteObject);
			return new XElement(ELEMENT_CHANNELS, elements);
		}

		public IEnumerable<Channel> ReadObject(XElement element) {
			List<Channel> channels = new List<Channel>();

			XElement parentNode = element.Element(ELEMENT_CHANNELS);
			if(parentNode != null) {
				XmlChannelSerializer channelSerializer = new XmlChannelSerializer();
				channels.AddRange(parentNode.Elements().Select(channelSerializer.ReadObject).NotNull());
			}

			return channels;
		}
	}
}
