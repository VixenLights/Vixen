using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Vixen.Sys;
using Vixen.Sys.Output;

namespace Vixen.IO.Xml.Serializer
{
	internal class XmlSmartControllerCollectionSerializer : IXmlSerializer<IEnumerable<IOutputDevice>>
	{
		private const string ELEMENT_SMART_CONTROLLERS = "SmartControllers";

		public XElement WriteObject(IEnumerable<IOutputDevice> value)
		{
			XmlSmartControllerSerializer serializer = new XmlSmartControllerSerializer();
			IEnumerable<XElement> elements = value.Select(serializer.WriteObject);
			return new XElement(ELEMENT_SMART_CONTROLLERS, elements);
		}

		public IEnumerable<IOutputDevice> ReadObject(XElement element)
		{
			List<IOutputDevice> controllers = new List<IOutputDevice>();

			XElement parentNode = element.Element(ELEMENT_SMART_CONTROLLERS);
			if (parentNode != null) {
				XmlSmartControllerSerializer serializer = new XmlSmartControllerSerializer();
				controllers.AddRange(parentNode.Elements().Select(serializer.ReadObject).Where(x => x != null));
			}

			return controllers;
		}
	}
}