using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Vixen.Sys;

namespace Vixen.IO.Xml.Serializer {
	class XmlElementNodeCollectionSerializer : IXmlSerializer<IEnumerable<ElementNode>> {
		private IEnumerable<Element> _elements;

		private const string ELEMENT_NODES = "Nodes";

		public XmlElementNodeCollectionSerializer(IEnumerable<Element> elements) {
			_elements = elements;
		}

		public XElement WriteObject(IEnumerable<ElementNode> value) {
			XmlElementNodeSerializer elementNodeSerializer = new XmlElementNodeSerializer(_elements);
			IEnumerable<XElement> elements = value.Select(elementNodeSerializer.WriteObject);
			return new XElement(ELEMENT_NODES, elements);
		}

		public IEnumerable<ElementNode> ReadObject(XElement element) {
			// Any references to non-existent elements will be pruned by this operation.
			List<ElementNode> elementNodes = new List<ElementNode>();

			XElement parentNode = element.Element(ELEMENT_NODES);
			if(parentNode != null) {
				XmlElementNodeSerializer elementNodeSerializer = new XmlElementNodeSerializer(_elements);
				IEnumerable<ElementNode> childNodes = parentNode.Elements().Select(elementNodeSerializer.ReadObject).NotNull();
				elementNodes.AddRange(childNodes);
			}

			return elementNodes;
		}
	}
}
