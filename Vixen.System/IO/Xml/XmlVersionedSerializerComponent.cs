using System.Xml.Linq;

namespace Vixen.IO.Xml {
	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="T">Type of object being serialized.</typeparam>
	//*** move the two methods into the base class, hide the type of serialized data, and get rid
	//    of this class
	abstract class XmlVersionedSerializerComponent<T> : VersionedSerializerComponent<T, XElement>
		where T : class {
		private const string ATTR_VERSION = "version";

		protected int GetVersion(XElement element) {
			int version = 1;

			XAttribute attribute = element.Attribute(ATTR_VERSION);
			if(attribute != null) {
				int.TryParse(attribute.Value, out version);
			}

			return version;
		}

		protected void PutVersion(XElement element, int version) {
			XAttribute versionAttribute = element.Attribute(ATTR_VERSION);
			if(versionAttribute != null) {
				versionAttribute.SetValue(version);
			} else {
				element.Add(new XAttribute(ATTR_VERSION, version));
			}
		}
	}
}
