using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Vixen.Module.PostFilter;
using Vixen.Sys;

namespace Vixen.IO.Xml {
	class XmlPostFilterCollectionSerializer : IXmlSerializer<IEnumerable<IPostFilterModuleInstance>> {
		private const string ELEMENT_FILTERS = "FilterNodes";
		private const string ELEMENT_FILTER = "FilterNode";
		private const string ATTR_TYPE_ID = "typeId";
		private const string ATTR_INSTANCE_ID = "instanceId";

		public XElement WriteObject(IEnumerable<IPostFilterModuleInstance> value) {
			return new XElement(ELEMENT_FILTERS, value.Select(x =>
									new XElement(ELEMENT_FILTER,
										new XAttribute(ATTR_TYPE_ID, x.Descriptor.TypeId),
										new XAttribute(ATTR_INSTANCE_ID, x.InstanceId))));
		}

		public IEnumerable<IPostFilterModuleInstance> ReadObject(XElement element) {
			List<IPostFilterModuleInstance> postFilters = new List<IPostFilterModuleInstance>();
			
			XElement filterElements = element.Element(ELEMENT_FILTERS);
			if(filterElements != null) {
				foreach(XElement filterElement in filterElements.Elements(ELEMENT_FILTER)) {
					Guid? typeId = XmlHelper.GetGuidAttribute(filterElement, ATTR_TYPE_ID);
					if(typeId == null) continue;

					Guid? instanceId = XmlHelper.GetGuidAttribute(filterElement, ATTR_INSTANCE_ID);
					if(instanceId == null) continue;

					IPostFilterModuleInstance postFilter = Modules.ModuleManagement.GetPostFilter(typeId);
					postFilter.InstanceId = instanceId.Value;

					postFilters.Add(postFilter);
				}
			}

			return postFilters;
		}
	}
}
