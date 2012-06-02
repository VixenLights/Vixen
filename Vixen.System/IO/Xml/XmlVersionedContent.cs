using System.Xml.Linq;

namespace Vixen.IO.Xml {
	internal class XmlVersionedContent : XElement {
		private const string ATTR_VERSION = "version";

		public XmlVersionedContent(XName name)
			: base(name) {
		}

		public XmlVersionedContent(XElement content)
			: base(content) {
		}

		public int Version {
			get {
				XAttribute versionAttribute = Attribute(ATTR_VERSION);
				int value = 0;
				if(versionAttribute != null) {
					int.TryParse(versionAttribute.Value, out value);
				}
				return value;
			}
			set {
				XAttribute versionAttribute = Attribute(ATTR_VERSION);
				if(versionAttribute == null) {
					versionAttribute = new XAttribute(ATTR_VERSION, 0);
					Add(versionAttribute);
				}
				versionAttribute.Value = value.ToString();
			}
		}
	}
}