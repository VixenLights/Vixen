using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Vixen.Module.PostFilter;
using Vixen.Sys;
using Vixen.Sys.Output;

namespace Vixen.IO.Xml {
	class XmlControllerSerializer : IXmlSerializer<IOutputDevice> {
		private const string ELEMENT_CONTROLLER = "Controller";
		private const string ELEMENT_OUTPUTS = "Outputs";
		private const string ELEMENT_OUTPUT = "Output";
		private const string ATTR_NAME = "name";
		private const string ATTR_HARDWARE_ID = "hardwareId";
		private const string ATTR_OUTPUT_COUNT = "outputCount";
		private const string ATTR_ID = "id";

		public XElement WriteObject(IOutputDevice value) {
			OutputController controller = (OutputController)value;
			//controller.Commit();

			XmlModuleLocalDataSetSerializer dataSetSerializer = new XmlModuleLocalDataSetSerializer();
			XElement dataSetElement = null;
			if(controller.ModuleDataSet != null) {
				dataSetElement = dataSetSerializer.WriteObject(controller.ModuleDataSet);
			}

			XmlPostFilterCollectionSerializer postFilterCollectionSerializer = new XmlPostFilterCollectionSerializer();

			XElement element = new XElement(ELEMENT_CONTROLLER,
				new XAttribute(ATTR_NAME, controller.Name),
				new XAttribute(ATTR_HARDWARE_ID, controller.ModuleId),
				new XAttribute(ATTR_OUTPUT_COUNT, controller.OutputCount),
				new XAttribute(ATTR_ID, controller.Id),

				dataSetElement,
				new XElement(ELEMENT_OUTPUTS,
					controller.Outputs.Select((x, index) =>
						new XElement(ELEMENT_OUTPUT,
							new XAttribute(ATTR_NAME, x.Name),
							postFilterCollectionSerializer.WriteObject(x.PostFilters)))));

			return element;
		}

		public IOutputDevice ReadObject(XElement element) {
			string name = XmlHelper.GetAttribute(element, ATTR_NAME);
			if(name == null) return null;

			Guid? outputModuleId = XmlHelper.GetGuidAttribute(element, ATTR_HARDWARE_ID);
			if(outputModuleId == null) return null;

			int? outputCount = XmlHelper.GetIntAttribute(element, ATTR_OUTPUT_COUNT);

			Guid? id = XmlHelper.GetGuidAttribute(element, ATTR_ID);
			if(id == null) return null;

			OutputController controller = new OutputController(id.Value, name, outputCount.GetValueOrDefault(), outputModuleId.Value);

			_PopulateController(controller, element);

			return controller;
		}

		private void _PopulateController(OutputController controller, XElement element) {
			XmlModuleLocalDataSetSerializer dataSetSerializer = new XmlModuleLocalDataSetSerializer();
			controller.ModuleDataSet = dataSetSerializer.ReadObject(element);

			XElement outputsElement = element.Element(ELEMENT_OUTPUTS);
			if(outputsElement != null) {
				int outputIndex = 0;
				foreach(XElement outputElement in outputsElement.Elements(ELEMENT_OUTPUT)) {
					// Data persisted in the controller instance may exceed the
					// output count.
					if(outputIndex >= controller.OutputCount) break;

					// The outputs were created when the output count was set.
					CommandOutput output = controller.Outputs[outputIndex];

					output.Name = XmlHelper.GetAttribute(outputElement, ATTR_NAME) ?? "Unnamed output";

					XmlPostFilterCollectionSerializer postFilterCollectionSerializer = new XmlPostFilterCollectionSerializer();
					IEnumerable<IPostFilterModuleInstance> postFilters = postFilterCollectionSerializer.ReadObject(outputElement);
					controller.ClearPostFilters(outputIndex);
					foreach(IPostFilterModuleInstance postFilter in postFilters) {
						controller.AddPostFilter(outputIndex, postFilter);
					}

					outputIndex++;
				}
			}
		}
	}
}
