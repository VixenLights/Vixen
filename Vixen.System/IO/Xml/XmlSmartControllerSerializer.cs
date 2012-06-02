using System;
using System.Linq;
using System.Xml.Linq;
using Vixen.Sys;
using Vixen.Sys.Output;

namespace Vixen.IO.Xml {
	class XmlSmartControllerSerializer : IXmlSerializer<IOutputDevice> {
		private const string ELEMENT_SMART_CONTROLLER = "SmartController";
		private const string ELEMENT_OUTPUTS = "Outputs";
		private const string ELEMENT_OUTPUT = "Output";
		private const string ATTR_NAME = "name";
		private const string ATTR_HARDWARE_ID = "hardwareId";
		private const string ATTR_OUTPUT_COUNT = "outputCount";
		private const string ATTR_ID = "id";

		public XElement WriteObject(IOutputDevice value) {
			SmartOutputController controller = (SmartOutputController)value;

			XmlModuleLocalDataSetSerializer dataSetSerializer = new XmlModuleLocalDataSetSerializer();
			XElement dataSetElement = null;
			if(controller.ModuleDataSet != null) {
				dataSetElement = dataSetSerializer.WriteObject(controller.ModuleDataSet);
			}

			XElement element = new XElement(ELEMENT_SMART_CONTROLLER,
				new XAttribute(ATTR_NAME, controller.Name),
				new XAttribute(ATTR_HARDWARE_ID, controller.ModuleId),
				new XAttribute(ATTR_OUTPUT_COUNT, controller.OutputCount),
				new XAttribute(ATTR_ID, controller.Id),
				dataSetElement,
				_WriteOutputNames(controller));

			return element;
		}

		public IOutputDevice ReadObject(XElement element) {
			string name = XmlHelper.GetAttribute(element, ATTR_NAME);
			if(name == null) return null;

			Guid? moduleId = XmlHelper.GetGuidAttribute(element, ATTR_HARDWARE_ID);
			if(moduleId == null) return null;

			int? outputCount = XmlHelper.GetIntAttribute(element, ATTR_OUTPUT_COUNT);

			Guid? id = XmlHelper.GetGuidAttribute(element, ATTR_ID);
			if(id == null) return null;

			SmartOutputController controller = new SmartOutputController(id.Value, name, outputCount.GetValueOrDefault(), moduleId.Value);

			XmlModuleLocalDataSetSerializer dataSetSerializer = new XmlModuleLocalDataSetSerializer();
			controller.ModuleDataSet = dataSetSerializer.ReadObject(element);

			_ReadOutputNames(controller, element);

			return controller;
		}

		private XElement _WriteOutputNames(SmartOutputController controller) {
			return new XElement(ELEMENT_OUTPUTS,
					controller.Outputs.Select((x, index) =>
						new XElement(ELEMENT_OUTPUT,
							new XAttribute(ATTR_NAME, x.Name))));
		}

		private void _ReadOutputNames(SmartOutputController controller, XElement element) {
			XElement outputsElement = element.Element(ELEMENT_OUTPUTS);
			if(outputsElement != null) {
				int outputIndex = 0;
				foreach(XElement outputElement in outputsElement.Elements(ELEMENT_OUTPUT)) {
					// Data persisted in the controller instance may exceed the
					// output count.
					if(outputIndex >= controller.OutputCount) break;

					// The outputs were created when the output count was set.
					IntentOutput output = controller.Outputs[outputIndex];

					output.Name = XmlHelper.GetAttribute(outputElement, ATTR_NAME) ?? "Unnamed output";

					outputIndex++;
				}
			}
		}
	}
}
