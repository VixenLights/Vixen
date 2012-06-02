using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Vixen.IO.Xml {
	class XmlPropertyCollectionSerializer : IXmlSerializer<IEnumerable<Guid>> {
		private const string ELEMENT_PROPERTIES = "Properties";
		private const string ELEMENT_PROPERTY = "Property";

		public XElement WriteObject(IEnumerable<Guid> value) {
			return new XElement(ELEMENT_PROPERTIES,
				value.Select(x => new XElement(ELEMENT_PROPERTY, x)));
		}

		public IEnumerable<Guid> ReadObject(XElement element) {
			element = element.Element(ELEMENT_PROPERTIES);
			if(element != null) {
				return element.Elements(ELEMENT_PROPERTY).Select(x => Guid.Parse(x.Value));
				//node.Properties.Add(new Guid(propertyElement.Value));
			}
			return Enumerable.Empty<Guid>();
		}
	}
}
