using System.Xml.Linq;
using Vixen.Execution;

namespace Vixen.IO.Xml {
	class XmlSelectedTimingProviderSerializer : IXmlSerializer<SelectedTimingProvider> {
		private const string ELEMENT_TIMING_SOURCE = "TimingSource";
		private const string ELEMENT_SELECTED_TIMING = "Selected";
		private const string ATTR_SELECTED_TIMING_TYPE = "type";
		private const string ATTR_SELECTED_TIMING_SOURCE = "source";

		public XElement WriteObject(SelectedTimingProvider value) {
			return new XElement(ELEMENT_TIMING_SOURCE,
				new XElement(ELEMENT_SELECTED_TIMING,
					new XAttribute(ATTR_SELECTED_TIMING_TYPE, (value != null) ? (value.ProviderType ?? string.Empty) : string.Empty),
					new XAttribute(ATTR_SELECTED_TIMING_SOURCE, (value != null) ? (value.SourceName ?? string.Empty) : string.Empty)));
		}

		public SelectedTimingProvider ReadObject(XElement element) {
			string providerType = null;
			string sourceName = null;

			element = element.Element(ELEMENT_TIMING_SOURCE);
			if(element != null) {
				element = element.Element(ELEMENT_SELECTED_TIMING);
				if(element != null) {
					XAttribute attribute = element.Attribute(ATTR_SELECTED_TIMING_TYPE);
					if(attribute != null) {
						providerType = attribute.Value;
						attribute = element.Attribute(ATTR_SELECTED_TIMING_SOURCE);
						if(attribute != null) {
							sourceName = attribute.Value;
							if(providerType.Length == 0) providerType = null;
							if(sourceName.Length == 0) sourceName = null;
						}
					}
				}
			}

			return new SelectedTimingProvider(providerType, sourceName);
		}
	}
}
