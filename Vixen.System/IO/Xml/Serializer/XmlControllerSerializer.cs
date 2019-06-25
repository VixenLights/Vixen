using System;
using System.Linq;
using System.Xml.Linq;
using Vixen.Factory;
using Vixen.Sys;
using Vixen.Sys.Output;

namespace Vixen.IO.Xml.Serializer
{
	internal class XmlControllerSerializer : IXmlSerializer<IOutputDevice>
	{
		private static NLog.Logger logging = NLog.LogManager.GetCurrentClassLogger();

		private const string ELEMENT_CONTROLLER = "Controller";
		private const string ELEMENT_OUTPUTS = "Outputs";
		private const string ELEMENT_OUTPUT = "Output";
		private const string ATTR_NAME = "name";
		private const string ATTR_HARDWARE_ID = "hardwareId";
		private const string ATTR_HARDWARE_INSTANCE_ID = "hardwareInstanceId";
		private const string ATTR_DEVICE_ID = "id";

		public XElement WriteObject(IOutputDevice value)
		{
			OutputController controller = (OutputController) value;

			XElement element = new XElement(ELEMENT_CONTROLLER,
			                                new XAttribute(ATTR_NAME, controller.Name),
			                                new XAttribute(ATTR_HARDWARE_ID, controller.ModuleId),
			                                new XAttribute(ATTR_HARDWARE_INSTANCE_ID, controller.ModuleInstanceId),
			                                new XAttribute(ATTR_DEVICE_ID, controller.Id),
											_WriteOutputs(controller));

			return element;
		}

		public IOutputDevice ReadObject(XElement element)
		{
			try {
				string name = XmlHelper.GetAttribute(element, ATTR_NAME);
				if (name == null)
					return null;

				Guid? moduleTypeId = XmlHelper.GetGuidAttribute(element, ATTR_HARDWARE_ID);
				if (moduleTypeId == null)
					return null;

				Guid? moduleInstanceId = XmlHelper.GetGuidAttribute(element, ATTR_HARDWARE_INSTANCE_ID);
				if (moduleInstanceId == null)
					return null;

				Guid? deviceId = XmlHelper.GetGuidAttribute(element, ATTR_DEVICE_ID);
				if (deviceId == null)
					return null;

				ControllerFactory controllerFactory = new ControllerFactory();
				OutputController controller =
					(OutputController) controllerFactory.CreateDevice(deviceId.Value, moduleTypeId.Value, moduleInstanceId.Value, name);

				_ReadOutputs(controller, element);

				return controller;
			} catch (Exception e) {
				logging.Error(e, "Error loading Controller from XML");
				return null;
			}
		}

		private XElement _WriteOutputs(OutputController controller)
		{
			return new XElement(ELEMENT_OUTPUTS,
			                    controller.Outputs.Select((x, index) =>
			                                              new XElement(ELEMENT_OUTPUT,
			                                                           new XAttribute(ATTR_NAME, x.Name),
			                                                           new XAttribute(ATTR_DEVICE_ID, x.Id))));
		}

		private void _ReadOutputs(OutputController controller, XElement element)
		{
			XElement outputsElement = element.Element(ELEMENT_OUTPUTS);
			if (outputsElement != null) {
				int index = 0;
				foreach (XElement outputElement in outputsElement.Elements(ELEMENT_OUTPUT)) {
					Guid? id = XmlHelper.GetGuidAttribute(outputElement, ATTR_DEVICE_ID);
					string name = XmlHelper.GetAttribute(outputElement, ATTR_NAME) ?? "Unnamed output";

					CommandOutputFactory outputFactory = new CommandOutputFactory();
					CommandOutput output = (CommandOutput) outputFactory.CreateOutput(id.GetValueOrDefault(), name, index++);

					controller.AddOutput(output);
				}
			}
		}
	}
}