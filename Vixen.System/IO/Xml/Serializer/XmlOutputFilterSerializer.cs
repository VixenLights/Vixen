using System;
using System.Xml.Linq;
using Vixen.Module.OutputFilter;
using Vixen.Sys;

namespace Vixen.IO.Xml.Serializer
{
	internal class XmlOutputFilterSerializer : IXmlSerializer<IOutputFilterModuleInstance>
	{
		private static NLog.Logger logging = NLog.LogManager.GetCurrentClassLogger();

		private const string ELEMENT_FILTER = "FilterNode";
		private const string ATTR_TYPE_ID = "typeId";
		private const string ATTR_INSTANCE_ID = "instanceId";

		public XElement WriteObject(IOutputFilterModuleInstance value)
		{
			return new XElement(ELEMENT_FILTER,
			                    new XAttribute(ATTR_TYPE_ID, value.Descriptor.TypeId),
			                    new XAttribute(ATTR_INSTANCE_ID, value.InstanceId));
		}

		public IOutputFilterModuleInstance ReadObject(XElement element)
		{
			try {
				Guid? typeId = XmlHelper.GetGuidAttribute(element, ATTR_TYPE_ID);
				if (typeId == null)
					return null;

				Guid? instanceId = XmlHelper.GetGuidAttribute(element, ATTR_INSTANCE_ID);
				if (instanceId == null)
					return null;

				IOutputFilterModuleInstance outputFilter = Modules.ModuleManagement.GetOutputFilter(typeId);
				if (outputFilter != null) {
					outputFilter.InstanceId = instanceId.Value;
				}

				return outputFilter;
			} catch (Exception e) {
				logging.Error(e, "Error loading Output Filter from XML");
				return null;
			}
		}
	}
}