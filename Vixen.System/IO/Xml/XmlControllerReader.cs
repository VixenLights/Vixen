using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Vixen.Hardware;
using Vixen.Common;
using Vixen.Module;
using Vixen.Module.Transform;

namespace Vixen.IO.Xml {
	class XmlControllerReader : IReader {
		private const string ELEMENT_ROOT = "Controller";
		private const string ELEMENT_OUTPUTS = "Outputs";
		private const string ELEMENT_OUTPUT = "Output";
		private const string ELEMENT_TRANSFORMS = "Transforms";
		private const string ELEMENT_TRANSFORM = "Transform";
		private const string ELEMENT_TRANSFORM_DATA = "TransformData";
		private const string ATTR_ID = "id";
		private const string ATTR_COMB_STRATEGY = "strategy";
		private const string ATTR_LINKED_TO = "linkedTo";
		private const string ATTR_OUTPUT_COUNT = "outputCount";
		private const string ATTR_HARDWARE_ID = "hardwareId";
		private const string ATTR_NAME = "name";
		private const string ATTR_TYPE_ID = "typeId";
		private const string ATTR_INSTANCE_ID = "instanceId";

		public object Read(string filePath) {
			XElement element = Helper.LoadXml(filePath);
			OutputController controller = CreateObject(element, filePath);
			return controller;
		}

		public OutputController CreateObject(XElement element, string filePath) {
			string name = System.IO.Path.GetFileNameWithoutExtension(filePath);
			Guid outputModuleId = new Guid(element.Attribute(ATTR_HARDWARE_ID).Value);
			int outputCount = int.Parse(element.Attribute(ATTR_OUTPUT_COUNT).Value);
			Guid id = Guid.Parse(element.Attribute(ATTR_ID).Value);
			Guid instanceId = Guid.NewGuid();
			CommandStandard.Standard.CombinationOperation combinationStrategy = (CommandStandard.Standard.CombinationOperation)Enum.Parse(typeof(CommandStandard.Standard.CombinationOperation), element.Attribute(ATTR_COMB_STRATEGY).Value);
	
			OutputController controller = new OutputController(id, instanceId, name, outputCount, outputModuleId, combinationStrategy);

			controller.LinkedTo = Guid.Parse(element.Attribute(ATTR_LINKED_TO).Value);

			if(controller.OutputModule != null) {
				controller.OutputModule.TransformModuleData = _GetTransformModuleData(element.Element(ELEMENT_TRANSFORM_DATA));
			}

			int outputIndex = 0;
			foreach(XElement outputElement in element.Element(ELEMENT_OUTPUTS).Elements(ELEMENT_OUTPUT)) {
				// Data persisted in the controller instance may exceed the
				// output count.
				if(outputIndex >= controller.OutputCount) break;

				// The outputs were created when the output count was set.
				OutputController.Output output = controller.Outputs[outputIndex];

				output.Name = outputElement.Attribute(ATTR_NAME).Value;

				if(controller.OutputModule != null) {
					// Create transform instances.
					foreach(XElement transformElement in outputElement.Element(ELEMENT_TRANSFORMS).Elements(ELEMENT_TRANSFORM)) {
						Guid moduleTypeId = Guid.Parse(transformElement.Attribute(ATTR_TYPE_ID).Value);
						Guid moduleInstanceId = Guid.Parse(transformElement.Attribute(ATTR_INSTANCE_ID).Value);
						controller.OutputModule.AddTransform(outputIndex, moduleTypeId, moduleInstanceId);
					}
				}

				outputIndex++;
			}

			return controller;
		}

		private IModuleDataSet _GetTransformModuleData(XElement element) {
			IModuleDataSet moduleDataSet = new ModuleDataSet();

			if(!element.IsEmpty) {
				string moduleDataString = element.InnerXml();
				moduleDataSet.Deserialize(moduleDataString);
			}

			return moduleDataSet;
		}
	}
}
