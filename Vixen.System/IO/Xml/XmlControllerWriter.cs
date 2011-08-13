using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Vixen.Hardware;

namespace Vixen.IO.Xml {
	class XmlControllerWriter : IWriter {
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

		public void Write(string filePath, object value) {
			if(!(value is OutputController)) throw new InvalidOperationException("Attempt to serialize a " + value.GetType().ToString() + " as an OutputController.");

			OutputController controller = (OutputController)value;
			XElement doc = CreateContent(controller);
			doc.Save(filePath);
		}

		public XElement CreateContent(OutputController controller) {
			controller.Commit();
			return new XElement(ELEMENT_ROOT,
				new XAttribute(ATTR_HARDWARE_ID, controller.OutputModuleId),
				new XAttribute(ATTR_OUTPUT_COUNT, controller.OutputCount),
				new XAttribute(ATTR_ID, controller.Id),
				new XAttribute(ATTR_COMB_STRATEGY, controller.CombinationStrategy),
				new XAttribute(ATTR_LINKED_TO, controller.LinkedTo),

				new XElement(ELEMENT_TRANSFORM_DATA, _CreateTransformModuleDataContent(controller)),
				new XElement(ELEMENT_OUTPUTS,
					controller.Outputs.Select((x, index) =>
						new XElement(ELEMENT_OUTPUT,
							new XAttribute(ATTR_NAME, x.Name),
							new XElement(ELEMENT_TRANSFORMS,
								_CreateOutputTransformContent(controller, index))))));
		}

		private XElement _CreateTransformModuleDataContent(OutputController controller) {
			if(controller.OutputModule != null) {
				return controller.OutputModule.TransformModuleData.ToXElement();
			}
			return null;
		}

		private IEnumerable<XElement> _CreateOutputTransformContent(OutputController controller, int outputIndex) {
			if(controller.OutputModule != null) {
				return controller.OutputModule.GetOutputTransforms(outputIndex).Select(x =>
					new XElement(ELEMENT_TRANSFORM,
						new XAttribute(ATTR_TYPE_ID, x.Descriptor.TypeId),
						new XAttribute(ATTR_INSTANCE_ID, x.InstanceId)));

			}
			return null;
		}
	}
}
