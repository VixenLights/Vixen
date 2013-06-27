using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Vixen.Sys;

namespace Vixen.IO.Xml.Serializer
{
	internal class XmlElementCollectionSerializer : IXmlSerializer<IEnumerable<Element>>
	{
		private const string ELEMENT_ELEMENTS = "Channels";

		public XElement WriteObject(IEnumerable<Element> value)
		{
			XmlElementSerializer elementSerializer = new XmlElementSerializer();
			IEnumerable<XElement> elements = value.Select(elementSerializer.WriteObject);
			return new XElement(ELEMENT_ELEMENTS, elements);
		}

		public IEnumerable<Element> ReadObject(XElement element)
		{
			List<Element> elements = new List<Element>();

			XElement parentNode = element.Element(ELEMENT_ELEMENTS);
			if (parentNode != null) {
				XmlElementSerializer elementSerializer = new XmlElementSerializer();
				elements.AddRange(parentNode.Elements().Select(elementSerializer.ReadObject).Where(x => x != null));
			}

			return elements;
		}
	}
}