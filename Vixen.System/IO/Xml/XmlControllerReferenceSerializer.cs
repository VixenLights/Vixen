using System;
using System.Xml.Linq;
using Vixen.Sys;

namespace Vixen.IO.Xml {
	class XmlControllerReferenceSerializer : IXmlSerializer<ControllerReference> {
		private const string ELEMENT_CONTROLLER_REFERENCE = "ControllerReference";
		private const string ATTR_CONTROLLER_ID = "controllerId";
		private const string ATTR_OUTPUT_INDEX = "outputIndex";

		public XElement WriteObject(ControllerReference value) {
			XElement element = new XElement(ELEMENT_CONTROLLER_REFERENCE,
				new XAttribute(ATTR_CONTROLLER_ID, value.ControllerId),
				new XAttribute(ATTR_OUTPUT_INDEX, value.OutputIndex));
			return element;
		}

		public ControllerReference ReadObject(XElement element) {
			Guid? controllerId = XmlHelper.GetGuidAttribute(element, ATTR_CONTROLLER_ID);
			if(controllerId == null) return null;

			int? outputIndex = XmlHelper.GetIntAttribute(element, ATTR_OUTPUT_INDEX);
			if(outputIndex == null) return null;

			return new ControllerReference(controllerId.Value, outputIndex.Value);
		}
	}
}
