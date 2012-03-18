using System;
using System.Xml.Linq;
using Vixen.Module.PostFilter;
using Vixen.Sys;

namespace Vixen.IO.Xml {
	class XmlPostFilterSerializer : IXmlSerializer<IPostFilterModuleInstance> {
		private const string ELEMENT_FILTER = "FilterNode";
		private const string ATTR_TYPE_ID = "typeId";
		private const string ATTR_INSTANCE_ID = "instanceId";

		public XElement WriteObject(IPostFilterModuleInstance value) {
			return new XElement(ELEMENT_FILTER,
				new XAttribute(ATTR_TYPE_ID, value.Descriptor.TypeId),
				new XAttribute(ATTR_INSTANCE_ID, value.InstanceId));
		}

		public IPostFilterModuleInstance ReadObject(XElement element) {
			Guid? typeId = XmlHelper.GetGuidAttribute(element, ATTR_TYPE_ID);
			if(typeId == null) return null;

			Guid? instanceId = XmlHelper.GetGuidAttribute(element, ATTR_INSTANCE_ID);
			if(instanceId == null) return null;

			IPostFilterModuleInstance postFilter = Modules.ModuleManagement.GetPostFilter(typeId);
			if(postFilter != null) {
				postFilter.InstanceId = instanceId.Value;
			}

			return postFilter;
		}
	}
}
