using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Vixen.IO.Xml
{
	internal static class XmlRootAttributeVersion
	{
		private const string ATTR_VERSION = "version";

		public static int GetVersion(XElement rootElement)
		{
			if (rootElement == null) throw new ArgumentNullException("rootElement");

			XAttribute versionAttr = rootElement.Attribute(ATTR_VERSION);
			if (versionAttr == null) return -1;

			int version;
			int.TryParse(versionAttr.Value, out version);
			return version;
		}

		public static void SetVersion(XElement rootElement, int version)
		{
			if (rootElement == null) throw new ArgumentNullException("rootElement");

			XAttribute versionAttribute = rootElement.Attribute(ATTR_VERSION);
			if (versionAttribute == null) {
				versionAttribute = new XAttribute(ATTR_VERSION, 0);
				rootElement.Add(versionAttribute);
			}

			versionAttribute.Value = version.ToString();
		}
	}
}