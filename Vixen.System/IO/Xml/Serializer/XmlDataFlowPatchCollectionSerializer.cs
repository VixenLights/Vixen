using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Vixen.Data.Flow;

namespace Vixen.IO.Xml.Serializer {
	class XmlDataFlowPatchCollectionSerializer : IXmlSerializer<IEnumerable<DataFlowPatch>> {
		private const string ELEMENT_PATCHES = "Patches";

		public XElement WriteObject(IEnumerable<DataFlowPatch> value) {
			XmlDataFlowPatchSerializer serializer = new XmlDataFlowPatchSerializer();
			IEnumerable<XElement> elements = value.Select(serializer.WriteObject);
			return new XElement(ELEMENT_PATCHES, elements);
		}

		public IEnumerable<DataFlowPatch> ReadObject(XElement source) {
			List<DataFlowPatch> patches = new List<DataFlowPatch>();

			XElement parentNode = source.Element(ELEMENT_PATCHES);
			if(parentNode != null) {
				XmlDataFlowPatchSerializer serializer = new XmlDataFlowPatchSerializer();
				patches.AddRange(parentNode.Elements().Select(serializer.ReadObject));
			}

			return patches;
		}
	}
}
