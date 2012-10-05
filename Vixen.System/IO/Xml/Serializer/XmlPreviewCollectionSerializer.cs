using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Vixen.Sys;
using Vixen.Sys.Output;

namespace Vixen.IO.Xml.Serializer {
	class XmlPreviewCollectionSerializer : IXmlSerializer<IEnumerable<IOutputDevice>> {
		private const string ELEMENT_PREVIEWS = "Previews";

		public XElement WriteObject(IEnumerable<IOutputDevice> value) {
			XmlPreviewSerializer previewSerializer = new XmlPreviewSerializer();
			IEnumerable<XElement> elements = value.Select(previewSerializer.WriteObject);
			return new XElement(ELEMENT_PREVIEWS, elements);
		}

		public IEnumerable<IOutputDevice> ReadObject(XElement element) {
			List<IOutputDevice> previews = new List<IOutputDevice>();

			XElement parentNode = element.Element(ELEMENT_PREVIEWS);
			if(parentNode != null) {
				XmlPreviewSerializer previewSerializer = new XmlPreviewSerializer();
				previews.AddRange(parentNode.Elements().Select(previewSerializer.ReadObject).NotNull());
			}

			return previews;
		}
	}
}
