using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Vixen.Module.PostFilter;
using Vixen.Sys;

namespace Vixen.IO.Xml {
	class XmlPostFilterCollectionSerializer : IXmlSerializer<PostFilterCollection> {
		private const string ELEMENT_FILTERS = "Filters";

		public XElement WriteObject(PostFilterCollection value) {
			XmlPostFilterSerializer postFilterSerializer = new XmlPostFilterSerializer();
			IEnumerable<XElement> elements = value.Select(postFilterSerializer.WriteObject);
			return new XElement(ELEMENT_FILTERS, elements);
		}

		public PostFilterCollection ReadObject(XElement element) {
			PostFilterCollection postFilters = new PostFilterCollection();
			
			XElement filtersElement = element.Element(ELEMENT_FILTERS);
			if(filtersElement != null) {
				XmlPostFilterSerializer postFilterSerializer = new XmlPostFilterSerializer();
				IEnumerable<IPostFilterModuleInstance> filters = filtersElement.Elements().Select(postFilterSerializer.ReadObject).NotNull();
				postFilters.AddRange(filters);
			}

			return postFilters;
		}
	}
}
