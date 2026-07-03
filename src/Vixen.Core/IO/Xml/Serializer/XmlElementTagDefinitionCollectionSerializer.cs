using System.Xml.Linq;
using Vixen.Sys;

namespace Vixen.IO.Xml.Serializer
{
	internal class XmlElementTagDefinitionCollectionSerializer : IXmlSerializer<IEnumerable<ElementTagDefinition>>
	{
		private const string ELEMENT_TAGS = "Tags";

		public XElement WriteObject(IEnumerable<ElementTagDefinition> value)
		{
			XmlElementTagDefinitionSerializer serializer = new XmlElementTagDefinitionSerializer();
			IEnumerable<XElement> elements = value.Select(serializer.WriteObject);
			return new XElement(ELEMENT_TAGS, elements);
		}

		public IEnumerable<ElementTagDefinition> ReadObject(XElement element)
		{
			List<ElementTagDefinition> tags = new List<ElementTagDefinition>();

			XElement tagsElement = element.Element(ELEMENT_TAGS);
			if (tagsElement != null) {
				tags.AddRange(ReadUnwrappedCollection(tagsElement));
			}

			return tags;
		}

		public IEnumerable<ElementTagDefinition> ReadUnwrappedCollection(XElement element)
		{
			List<ElementTagDefinition> tags = new List<ElementTagDefinition>();

			XmlElementTagDefinitionSerializer serializer = new XmlElementTagDefinitionSerializer();
			tags.AddRange(element.Elements().Select(serializer.ReadObject).Where(x => x != null));

			return tags;
		}
	}
}
