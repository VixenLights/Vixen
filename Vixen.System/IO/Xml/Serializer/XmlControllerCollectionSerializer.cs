using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Vixen.Sys;
using Vixen.Sys.Output;

namespace Vixen.IO.Xml.Serializer {
	class XmlControllerCollectionSerializer : IXmlSerializer<IEnumerable<IOutputDevice>> {
		private const string ELEMENT_CONTROLLERS = "Controllers";

		public XElement WriteObject(IEnumerable<IOutputDevice> value) {
			XmlControllerSerializer controllerSerializer = new XmlControllerSerializer();
			IEnumerable<XElement> elements = value.Select(controllerSerializer.WriteObject);
			return new XElement(ELEMENT_CONTROLLERS, elements);
		}

		public IEnumerable<IOutputDevice> ReadObject(XElement element) {
			List<IOutputDevice> controllers = new List<IOutputDevice>();

			XElement parentNode = element.Element(ELEMENT_CONTROLLERS);
			if(parentNode != null) {
				XmlControllerSerializer controllerSerializer = new XmlControllerSerializer();
				controllers.AddRange(parentNode.Elements().Select(controllerSerializer.ReadObject).Where(x => x != null));
			}
			
			return controllers;
		}
	}
}
