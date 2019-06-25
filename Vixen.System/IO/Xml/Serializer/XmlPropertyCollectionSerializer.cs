using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Vixen.Module.Property;
using Vixen.Sys;

namespace Vixen.IO.Xml.Serializer
{
	internal class XmlPropertyCollectionSerializer : IXmlSerializer<IEnumerable<IPropertyModuleInstance>>
	{
		private static NLog.Logger logging = NLog.LogManager.GetCurrentClassLogger();

		private const string ELEMENT_PROPERTIES = "Properties";
		private const string ELEMENT_PROPERTY = "Property";
		private const string ELEMENT_TYPE_ID = "typeId";
		private const string ELEMENT_INSTANCE_ID = "instanceId";

		public XElement WriteObject(IEnumerable<IPropertyModuleInstance> value)
		{
			if (value.Count() <= 0)
				return null;

			XElement result = new XElement(ELEMENT_PROPERTIES);
			foreach (IPropertyModuleInstance instance in value) {
				XElement elem = new XElement(ELEMENT_PROPERTY);
				elem.Add(new XAttribute(ELEMENT_TYPE_ID, instance.Descriptor.TypeId));
				elem.Add(new XAttribute(ELEMENT_INSTANCE_ID, instance.InstanceId));
				result.Add(elem);
			}
			return result;
		}

		public IEnumerable<IPropertyModuleInstance> ReadObject(XElement element)
		{
			try {
				List<IPropertyModuleInstance> properties = new List<IPropertyModuleInstance>();
				element = element.Element(ELEMENT_PROPERTIES);

				if (element == null)
					return properties;

				foreach (XElement prop in element.Elements(ELEMENT_PROPERTY)) {
					//figure out how to get props.
					Guid? typeId = XmlHelper.GetGuidAttribute(prop, ELEMENT_TYPE_ID);
					if (typeId == null)
						continue;

					Guid? instanceId = XmlHelper.GetGuidAttribute(prop, ELEMENT_INSTANCE_ID);
					if (instanceId == null)
						continue;

					IPropertyModuleInstance property = Modules.ModuleManagement.GetProperty(typeId);
					if (property != null) {
						property.InstanceId = instanceId.Value;
						properties.Add(property);
					}
				}

				return properties;
			} catch (Exception e) {
				logging.Error(e, "Error loading Property Collection from XML");
				return null;
			}
		}
	}
}