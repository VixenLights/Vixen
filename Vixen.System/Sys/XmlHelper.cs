using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

		static public long? GetLongAttribute(XElement element, string attributeName) {
			string attributeValue = GetAttribute(element, attributeName);
			return (attributeValue != null) ? long.Parse(attributeValue) : (long?)null;
		}

		static public TimeSpan? GetTimeSpanAttribute(XElement element, string attributeName) {
			long? attributeValue = GetLongAttribute(element, attributeName);
			return (attributeValue != null) ? TimeSpan.FromTicks(attributeValue.Value) : (TimeSpan?)null;
		}
	}
}
