using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Vixen.Module.OutputFilter;
using Vixen.Sys;

namespace Vixen.IO.Xml.Serializer {
	class XmlOutputFilterCollectionSerializer : IXmlSerializer<IEnumerable<IOutputFilterModuleInstance>> {
		private const string ELEMENT_FILTERS = "Filters";

		public XElement WriteObject(IEnumerable<IOutputFilterModuleInstance> value) {
			XmlOutputFilterSerializer serializer = new XmlOutputFilterSerializer();
			IEnumerable<XElement> elements = value.Select(serializer.WriteObject);
			return new XElement(ELEMENT_FILTERS, elements);
		}

		public IEnumerable<IOutputFilterModuleInstance> ReadObject(XElement element) {
			List<IOutputFilterModuleInstance> filters = new List<IOutputFilterModuleInstance>();

			XElement filtersElement = element.Element(ELEMENT_FILTERS);
			if(filtersElement != null) {
				filters.AddRange(ReadUnwrappedCollection(filtersElement));
			}

			return filters;
		}

		public IEnumerable<IOutputFilterModuleInstance> ReadUnwrappedCollection(XElement element) {
			List<IOutputFilterModuleInstance> filters = new List<IOutputFilterModuleInstance>();

			XmlOutputFilterSerializer serializer = new XmlOutputFilterSerializer();
			filters.AddRange(element.Elements().Select(serializer.ReadObject).NotNull());

			return filters;
		}
	}
	//class XmlOutputFilterCollectionSerializer : IXmlSerializer<OutputFilterCollection> {
	//    private const string ELEMENT_FILTERS = "Filters";

	//    public XElement WriteObject(OutputFilterCollection value) {
	//        XmlOutputFilterSerializer serializer = new XmlOutputFilterSerializer();
	//        IEnumerable<XElement> elements = value.Select(serializer.WriteObject);
	//        return new XElement(ELEMENT_FILTERS, elements);
	//    }

	//    public OutputFilterCollection ReadObject(XElement element) {
	//        OutputFilterCollection filters = new OutputFilterCollection();

	//        XElement filtersElement = element.Element(ELEMENT_FILTERS);
	//        if(filtersElement != null) {
	//            filters.AddRange(ReadUnwrappedCollection(filtersElement));
	//        }

	//        return filters;
	//    }

	//    public OutputFilterCollection ReadUnwrappedCollection(XElement element) {
	//        OutputFilterCollection filters = new OutputFilterCollection();

	//        XmlOutputFilterSerializer serializer = new XmlOutputFilterSerializer();
	//        filters.AddRange(element.Elements().Select(serializer.ReadObject).NotNull());

	//        return filters;
	//    }
	//}
}
