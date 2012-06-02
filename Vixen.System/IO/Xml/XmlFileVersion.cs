using System;
using System.Xml.Linq;

namespace Vixen.IO.Xml {
	class XmlFileVersion {
		public void PutVersion(XElement owningElement, string attributeName, int version) {
			XAttribute versionAttribute = owningElement.Attribute(attributeName);
			
			if(versionAttribute == null) {
				versionAttribute = new XAttribute(attributeName, version);
				owningElement.Add(versionAttribute);
			}

			versionAttribute.Value = version.ToString();
		}

		public int GetVersion(XElement owningElement, string attributeName) {
			XAttribute versionAttribute = owningElement.Attribute(attributeName);
			if(versionAttribute != null) {
				int version;
				if(int.TryParse(versionAttribute.Value, out version)) {
					return version;
				}
				throw new Exception("File version could not be determined.");
			}
			throw new Exception("File does not have a version.");
		}
	}
}
