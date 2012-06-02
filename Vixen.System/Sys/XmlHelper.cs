using System;
using System.Xml.Linq;

namespace Vixen.Sys {
	static class XmlHelper {
		static public string GetAttribute(XElement element, string attributeName) {
			XAttribute attribute = element.Attribute(attributeName);
			return (attribute != null) ? attribute.Value : null;
		}

		static public Guid? GetGuidAttribute(XElement element, string attributeName) {
			string attributeValue = GetAttribute(element, attributeName);
			return (attributeValue != null) ? Guid.Parse(attributeValue) : (Guid?)null;
		}

		static public int? GetIntAttribute(XElement element, string attributeName) {
			string attributeValue = GetAttribute(element, attributeName);
			return (attributeValue != null) ? int.Parse(attributeValue) : (int?)null;
		}

		static public long? GetLongAttribute(XElement element, string attributeName) {
			string attributeValue = GetAttribute(element, attributeName);
			return (attributeValue != null) ? long.Parse(attributeValue) : (long?)null;
		}

		static public TimeSpan? GetTimeSpanAttribute(XElement element, string attributeName) {
			long? attributeValue = GetLongAttribute(element, attributeName);
			return (attributeValue != null) ? TimeSpan.FromTicks(attributeValue.Value) : (TimeSpan?)null;
		}

		static public bool GetElementValue(XElement contentElement, string childElementName, bool defaultValue) {
			bool value = defaultValue;

			XElement element = contentElement.Element(childElementName);
			if(element != null) {
				bool.TryParse(element.Value, out value);
			}

			return value;
		}
	}
}
