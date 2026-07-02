using System.Xml.Linq;
using Vixen.Sys;

namespace Vixen.IO.Xml.Serializer
{
	internal class XmlElementTagDefinitionSerializer : IXmlSerializer<ElementTagDefinition>
	{
		private const string ELEMENT_TAG = "Tag";
		private const string ATTR_ID = "id";
		private const string ATTR_KEY = "key";
		private const string ATTR_NAME = "name";
		private const string ATTR_IS_BUILT_IN = "isBuiltIn";
		private const string ATTR_SORT_ORDER = "sortOrder";
		private const string ATTR_DESCRIPTION = "description";
		private const string ATTR_DISPLAY_COLOR = "displayColor";

		public XElement WriteObject(ElementTagDefinition value)
		{
			XElement element = new XElement(ELEMENT_TAG,
			                                new XAttribute(ATTR_ID, value.Id),
			                                new XAttribute(ATTR_KEY, value.Key),
			                                new XAttribute(ATTR_NAME, value.Name));
			
			if (value.IsBuiltIn) {
				element.Add(new XAttribute(ATTR_IS_BUILT_IN, value.IsBuiltIn));
			}
			
			//Optional fields
			if (!string.IsNullOrEmpty(value.Description)) {
				element.Add(new XAttribute(ATTR_DESCRIPTION, value.Description));
			}

			if (!string.IsNullOrEmpty(value.DisplayColor)) {
				element.Add(new XAttribute(ATTR_DISPLAY_COLOR, value.DisplayColor));
			}
			
			if (value.SortOrder > 0) {
				element.Add(new XAttribute(ATTR_SORT_ORDER, value.SortOrder));
			}

			return element;
		}

		public ElementTagDefinition ReadObject(XElement element)
		{
			Guid? id = XmlHelper.GetGuidAttribute(element, ATTR_ID);
			if (id == null)
				return null;

			string key = XmlHelper.GetAttribute(element, ATTR_KEY) ?? string.Empty;
			string name = XmlHelper.GetAttribute(element, ATTR_NAME) ?? string.Empty;
			bool isBuiltIn = bool.TryParse(XmlHelper.GetAttribute(element, ATTR_IS_BUILT_IN), out bool parsedIsBuiltIn) && parsedIsBuiltIn;

			ElementTagDefinition tag = new ElementTagDefinition(id.Value, key, name, isBuiltIn);

			int? sortOrder = XmlHelper.GetIntAttribute(element, ATTR_SORT_ORDER);
			tag.SortOrder = sortOrder ?? 0;

			tag.Description = XmlHelper.GetAttribute(element, ATTR_DESCRIPTION);
			tag.DisplayColor = XmlHelper.GetAttribute(element, ATTR_DISPLAY_COLOR);

			return tag;
		}
	}
}
