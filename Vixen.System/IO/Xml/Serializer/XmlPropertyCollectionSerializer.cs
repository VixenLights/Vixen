using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Vixen.Module.Property;
using Vixen.Sys;

namespace Vixen.IO.Xml.Serializer {
	class XmlPropertyCollectionSerializer : IXmlSerializer<IEnumerable<IPropertyModuleInstance>> {
		private const string ELEMENT_PROPERTIES = "Properties";
		private const string ELEMENT_PROPERTY = "Property";
		private const string ELEMENT_TYPE_ID = "typeId";
		private const string ELEMENT_INSTANCE_ID = "instanceId";

		public XElement WriteObject(IEnumerable<IPropertyModuleInstance> value)
		{
			return new XElement(ELEMENT_PROPERTIES,
				new XElement(ELEMENT_PROPERTY, 
					value.Select(x => new XAttribute(ELEMENT_TYPE_ID, x.Descriptor.TypeId)),
					value.Select(x => new XAttribute(ELEMENT_INSTANCE_ID, x.InstanceId)))
				);
		}

		//public XElement WriteObject(IEnumerable<Guid> value) {

		//    return new XElement(ELEMENT_PROPERTIES,
		//        value.Select(x => new XElement(ELEMENT_PROPERTY, x)));
		//}

		//public IEnumerable<Guid> ReadObject(XElement element)
		//{
		//    element = element.Element(ELEMENT_PROPERTIES);
		//    if (element != null)
		//    {
		//        return element.Elements(ELEMENT_PROPERTY).Select(x => Guid.Parse(x.Value));
		//    }
		//    return Enumerable.Empty<Guid>();
		//}

		public IEnumerable<IPropertyModuleInstance> ReadObject(XElement element)
		{
			List<IPropertyModuleInstance> properties = new List<IPropertyModuleInstance>();
			element = element.Element(ELEMENT_PROPERTIES);
			foreach (XElement prop in element.Elements(ELEMENT_PROPERTY))
			{
				//figure out how to get props.
				Guid? typeId = XmlHelper.GetGuidAttribute(prop, ELEMENT_TYPE_ID);
				if (typeId == null) continue;

				Guid? instanceId = XmlHelper.GetGuidAttribute(prop, ELEMENT_INSTANCE_ID);
				if (instanceId == null) continue;

				IPropertyModuleInstance property = Modules.ModuleManagement.GetProperty(typeId);
				if (property != null)
				{
					property.InstanceId = instanceId.Value;
					properties.Add(property);
				}
			}
			
			return properties;
		}
	}
}
