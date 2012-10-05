using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Vixen.Sys;

namespace Vixen.IO.Xml.Serializer {
	class XmlDisabledControllerCollectionSerializer : IXmlSerializer<IEnumerable<Guid>> {
		private const string ELEMENT_DISABLED_CONTROLLERS = "DisabledControllers";
		private const string ELEMENT_CONTROLLER = "Controller";
		private const string ATTR_ID = "id";

		public XElement WriteObject(IEnumerable<Guid> value) {
			IEnumerable<XElement> elements = value.Select(_WriteDisabledController);
			return new XElement(ELEMENT_DISABLED_CONTROLLERS, elements);
		}

		public IEnumerable<Guid> ReadObject(XElement element) {
			List<Guid> ids = new List<Guid>();

			XElement parentNode = element.Element(ELEMENT_DISABLED_CONTROLLERS);
			if(parentNode != null) {
				ids.AddRange(parentNode.Elements().Select(_ReadDisabledController).Where(x => x != null).Select(x => x.GetValueOrDefault()));
			}

			return ids;
		}

		private XElement _WriteDisabledController(Guid controllerId) {
			XElement element = new XElement(ELEMENT_CONTROLLER,
				new XAttribute(ATTR_ID, controllerId));
			return element;
		}

		private Guid? _ReadDisabledController(XElement element) {
			return XmlHelper.GetGuidAttribute(element, ATTR_ID);
		}
	}
}
