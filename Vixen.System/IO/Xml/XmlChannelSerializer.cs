using System;
using System.Xml.Linq;
using Vixen.Sys;

namespace Vixen.IO.Xml {
	class XmlChannelSerializer : IXmlSerializer<Channel> {
		private const string ELEMENT_CHANNEL = "Channel";
		private const string ATTR_ID = "id";
		private const string ATTR_NAME = "name";

		public XElement WriteObject(Channel value) {
			XElement element = new XElement(ELEMENT_CHANNEL,
				new XAttribute(ATTR_ID, value.Id),
				new XAttribute(ATTR_NAME, value.Name));
			return element;
		}

		public Channel ReadObject(XElement element) {
			Guid id = XmlHelper.GetGuidAttribute(element, ATTR_ID).GetValueOrDefault();
			string name = XmlHelper.GetAttribute(element, ATTR_NAME);
			return new Channel(id, name);
		}
	}
}
