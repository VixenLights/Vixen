using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Vixen.Sys;
using Vixen.Sys.Output;

namespace Vixen.IO.Xml {
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
				controllers.AddRange(parentNode.Elements().Select(controllerSerializer.ReadObject).NotNull());
			}
			
			return controllers;
		}

		//private XElement _WriteController(IOutputDevice controllerDevice) {
		//    OutputController controller = (OutputController)controllerDevice;
		//    //controller.Commit();

		//    XmlModuleLocalDataSetSerializer dataSetSerializer = new XmlModuleLocalDataSetSerializer();
		//    XElement dataSetElement = null;
		//    if(controller.OutputModule != null) {
		//        dataSetElement = dataSetSerializer.WriteObject(controller.OutputModule.ModuleDataSet);
		//    }

		//    XmlPostFilterCollectionSerializer postFilterCollectionSerializer = new XmlPostFilterCollectionSerializer();

		//    XElement element = new XElement(ELEMENT_CONTROLLER,
		//        new XAttribute(ATTR_NAME, controller.Name),
		//        new XAttribute(ATTR_HARDWARE_ID, controller.OutputModuleId),
		//        new XAttribute(ATTR_OUTPUT_COUNT, controller.OutputCount),
		//        new XAttribute(ATTR_ID, controller.Id),

		//        dataSetElement,
		//        new XElement(ELEMENT_OUTPUTS,
		//            controller.Outputs.Select((x, index) =>
		//                new XElement(ELEMENT_OUTPUT,
		//                    new XAttribute(ATTR_NAME, x.Name),
		//                    postFilterCollectionSerializer.WriteObject(x.PostFilters)))));

		//    return element;
		//}

		//private OutputController _ReadController(XElement element) {
		//    string name = XmlHelper.GetAttribute(element, ATTR_NAME);
		//    if(name == null) return null;

		//    Guid? outputModuleId = XmlHelper.GetGuidAttribute(element, ATTR_HARDWARE_ID);
		//    if(outputModuleId == null) return null;

		//    int? outputCount = XmlHelper.GetIntAttribute(element, ATTR_OUTPUT_COUNT);

		//    Guid? id = XmlHelper.GetGuidAttribute(element, ATTR_ID);
		//    if(id == null) return null;

		//    OutputController controller = new OutputController(id.Value, name, outputCount.GetValueOrDefault(), outputModuleId.Value);

		//    _PopulateController(controller, element);

		//    return controller;
		//}

		//private void _PopulateController(OutputController controller, XElement element) {
		//    XmlModuleLocalDataSetSerializer dataSetSerializer = new XmlModuleLocalDataSetSerializer();
		//    controller.ModuleDataSet = dataSetSerializer.ReadObject(element);

		//    XElement outputsElement = element.Element(ELEMENT_OUTPUTS);
		//    if(outputsElement != null) {
		//        int outputIndex = 0;
		//        foreach(XElement outputElement in outputsElement.Elements(ELEMENT_OUTPUT)) {
		//            // Data persisted in the controller instance may exceed the
		//            // output count.
		//            if(outputIndex >= controller.OutputCount) break;

		//            // The outputs were created when the output count was set.
		//            OutputController.Output output = controller.Outputs[outputIndex];

		//            output.Name = XmlHelper.GetAttribute(outputElement, ATTR_NAME) ?? "Unnamed output";

		//            XmlPostFilterCollectionSerializer postFilterCollectionSerializer = new XmlPostFilterCollectionSerializer();
		//            IEnumerable<IPostFilterModuleInstance> postFilters = postFilterCollectionSerializer.ReadObject(outputElement);
		//            controller.ClearPostFilters(outputIndex);
		//            foreach(IPostFilterModuleInstance postFilter in postFilters) {
		//                controller.AddPostFilter(outputIndex, postFilter);
		//            }

		//            outputIndex++;
		//        }
		//    }
		//}
	}
}
