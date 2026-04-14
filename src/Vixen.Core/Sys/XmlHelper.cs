using System.Xml.Linq;

namespace Vixen.Sys
{
	internal static class XmlHelper
	{
		public static string GetAttribute(XElement element, string attributeName)
		{
			XAttribute attribute = element.Attribute(attributeName);
			return (attribute != null) ? attribute.Value : null;
		}

		public static Guid? GetGuidAttribute(XElement element, string attributeName)
		{
			string attributeValue = GetAttribute(element, attributeName);
			return (attributeValue != null) ? Guid.Parse(attributeValue) : null;
		}

		public static int? GetIntAttribute(XElement element, string attributeName)
		{
			string attributeValue = GetAttribute(element, attributeName);
			return (attributeValue != null) ? int.Parse(attributeValue) : null;
		}

		public static long? GetLongAttribute(XElement element, string attributeName)
		{
			string attributeValue = GetAttribute(element, attributeName);
			return (attributeValue != null) ? long.Parse(attributeValue) : null;
		}

		public static TimeSpan? GetTimeSpanAttribute(XElement element, string attributeName)
		{
			long? attributeValue = GetLongAttribute(element, attributeName);
			return (attributeValue != null) ? TimeSpan.FromTicks(attributeValue.Value) : null;
		}

		public static bool GetElementValue(XElement contentElement, string childElementName, bool defaultValue)
		{
			bool value = defaultValue;

			XElement element = contentElement.Element(childElementName);
			if (element != null) {
				bool.TryParse(element.Value, out value);
			}

			return value;
		}
	}
}