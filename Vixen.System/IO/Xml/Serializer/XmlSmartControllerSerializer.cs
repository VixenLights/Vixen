using System;
using System.Linq;
using System.Xml.Linq;
using Vixen.Factory;
using Vixen.Sys;
using Vixen.Sys.Output;

namespace Vixen.IO.Xml.Serializer
{
	internal class XmlSmartControllerSerializer : IXmlSerializer<IOutputDevice>
	{
		private static NLog.Logger logging = NLog.LogManager.GetCurrentClassLogger();

		private const string ELEMENT_SMART_CONTROLLER = "SmartController";
		private const string ELEMENT_OUTPUTS = "Outputs";
		private const string ELEMENT_OUTPUT = "Output";
		private const string ATTR_NAME = "name";
		private const string ATTR_HARDWARE_ID = "hardwareId";
		private const string ATTR_HARDWARE_INSTANCE_ID = "hardwareInstanceId";
		private const string ATTR_ID = "id";

		public XElement WriteObject(IOutputDevice value)
		{
			SmartOutputController controller = (SmartOutputController) value;

			XElement element = new XElement(ELEMENT_SMART_CONTROLLER,
			                                new XAttribute(ATTR_NAME, controller.Name),
			                                new XAttribute(ATTR_HARDWARE_ID, controller.ModuleId),
			                                new XAttribute(ATTR_ID, controller.Id),
			                                _WriteOutputs(controller));

			return element;
		}

		public IOutputDevice ReadObject(XElement element)
		{
			try {
				string name = XmlHelper.GetAttribute(element, ATTR_NAME);
				if (name == null)
					return null;

				Guid? moduleId = XmlHelper.GetGuidAttribute(element, ATTR_HARDWARE_ID);
				if (moduleId == null)
					return null;

				Guid? moduleInstanceId = XmlHelper.GetGuidAttribute(element, ATTR_HARDWARE_INSTANCE_ID);
				if (moduleInstanceId == null)
					return null;

				Guid? deviceId = XmlHelper.GetGuidAttribute(element, ATTR_ID);
				if (deviceId == null)
					return null;

				SmartControllerFactory smartControllerFactory = new SmartControllerFactory();
				SmartOutputController controller =
				(SmartOutputController)
					smartControllerFactory.CreateDevice(deviceId.Value, moduleId.Value, moduleInstanceId.Value, name);

				_ReadOutputs(controller, element);

				return controller;
			} catch (Exception e) {
				logging.Error(e, "Error loading Smart Controller from XML");
				return null;
			}
		}

		private XElement _WriteOutputs(SmartOutputController controller)
		{
			return new XElement(ELEMENT_OUTPUTS,
			                    controller.Outputs.Select((x, index) =>
			                                              new XElement(ELEMENT_OUTPUT,
			                                                           new XAttribute(ATTR_NAME, x.Name),
			                                                           new XAttribute(ATTR_ID, x.Id))));
		}

		private void _ReadOutputs(SmartOutputController controller, XElement element)
		{
			XElement outputsElement = element.Element(ELEMENT_OUTPUTS);
			if (outputsElement != null) {
				int index = 0;
				foreach (XElement outputElement in outputsElement.Elements(ELEMENT_OUTPUT)) {
					Guid? id = XmlHelper.GetGuidAttribute(outputElement, ATTR_ID);
					string name = XmlHelper.GetAttribute(outputElement, ATTR_NAME) ?? "Unnamed output";

					IntentOutputFactory outputFactory = new IntentOutputFactory();
					IntentOutput output = (IntentOutput) outputFactory.CreateOutput(id.GetValueOrDefault(), name, index++);

					controller.AddOutput(output);
				}
			}
		}
	}
}