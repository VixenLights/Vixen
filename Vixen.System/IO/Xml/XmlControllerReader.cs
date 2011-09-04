using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Vixen.Hardware;
using Vixen.Sys;
using Vixen.Module;
using Vixen.Module.Transform;

namespace Vixen.IO.Xml {
	class XmlControllerReader : XmlReaderBase<OutputController> {
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

		override protected OutputController _CreateObject(XElement element, string filePath) {
			string name = System.IO.Path.GetFileNameWithoutExtension(filePath);
			Guid outputModuleId = new Guid(element.Attribute(ATTR_HARDWARE_ID).Value);
			int outputCount = int.Parse(element.Attribute(ATTR_OUTPUT_COUNT).Value);
			Guid id = Guid.Parse(element.Attribute(ATTR_ID).Value);
			Guid instanceId = Guid.NewGuid();
			CommandStandard.Standard.CombinationOperation combinationStrategy = (CommandStandard.Standard.CombinationOperation)Enum.Parse(typeof(CommandStandard.Standard.CombinationOperation), element.Attribute(ATTR_COMB_STRATEGY).Value);
	
			OutputController controller = new OutputController(id, instanceId, name, outputCount, outputModuleId, combinationStrategy);

			return controller;
		}

		private IModuleDataSet _GetTransformModuleData(XElement element) {
			IModuleDataSet moduleDataSet = new ModuleLocalDataSet();

			if(!element.IsEmpty) {
				string moduleDataString = element.InnerXml();
				moduleDataSet.Deserialize(moduleDataString);
			}

			return moduleDataSet;
		}

		protected override void _PopulateObject(OutputController obj, XElement element) {
			obj.LinkedTo = Guid.Parse(element.Attribute(ATTR_LINKED_TO).Value);

			if(obj.OutputModule != null) {
				obj.OutputModule.TransformModuleData = _GetTransformModuleData(element.Element(ELEMENT_TRANSFORM_DATA));
			}

			int outputIndex = 0;
			foreach(XElement outputElement in element.Element(ELEMENT_OUTPUTS).Elements(ELEMENT_OUTPUT)) {
				// Data persisted in the controller instance may exceed the
				// output count.
				if(outputIndex >= obj.OutputCount) break;

				// The outputs were created when the output count was set.
				OutputController.Output output = obj.Outputs[outputIndex];

				output.Name = outputElement.Attribute(ATTR_NAME).Value;

				if(obj.OutputModule != null) {
					// Create transform instances.
					foreach(XElement transformElement in outputElement.Element(ELEMENT_TRANSFORMS).Elements(ELEMENT_TRANSFORM)) {
						Guid moduleTypeId = Guid.Parse(transformElement.Attribute(ATTR_TYPE_ID).Value);
						Guid moduleInstanceId = Guid.Parse(transformElement.Attribute(ATTR_INSTANCE_ID).Value);
						obj.OutputModule.AddTransform(outputIndex, moduleTypeId, moduleInstanceId);
					}
				}

				outputIndex++;
			}
		}

		protected override IEnumerable<Func<XElement, XElement>> _ProvideMigrations(int versionAt, int targetVersion) {
			return new Func<XElement, XElement>[] { };
			//Testing
			//if(versionAt >= 1 && targetVersion <= 2) yield return _1_to_2;
		}

		//private XElement _1_to_2(XElement element) {
		//    element.SetAttributeValue("nerf", "herder");
		//    return element;
		//}
	}
}
