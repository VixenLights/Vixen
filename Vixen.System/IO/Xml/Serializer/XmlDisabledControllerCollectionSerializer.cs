using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Vixen.Sys;

namespace Vixen.IO.Xml.Serializer {
	class XmlDisabledControllerCollectionSerializer : IXmlSerializer<IEnumerable<Guid>> {
		private const string ELEMENT_DISABLED_DEVICES = "DisabledDevices";
		private const string ELEMENT_DEVICE = "Device";
		private const string ATTR_ID = "id";

		public XElement WriteObject(IEnumerable<Guid> value) {
			IEnumerable<XElement> elements = value.Select(_WriteDisabledDevice);
			return new XElement(ELEMENT_DISABLED_DEVICES, elements);
		}

		public IEnumerable<Guid> ReadObject(XElement element) {
			List<Guid> ids = new List<Guid>();

			XElement parentNode = element.Element(ELEMENT_DISABLED_DEVICES);
			if(parentNode != null) {
				ids.AddRange(parentNode.Elements().Select(_ReadDisabledDevice).Where(x => x != null).Select(x => x.GetValueOrDefault()));
			}

			return ids;
		}

		private XElement _WriteDisabledDevice(Guid id) {
			XElement element = new XElement(ELEMENT_DEVICE, new XAttribute(ATTR_ID, id));
			return element;
		}

		private Guid? _ReadDisabledDevice(XElement element) {
			return XmlHelper.GetGuidAttribute(element, ATTR_ID);
		}
	}
}
