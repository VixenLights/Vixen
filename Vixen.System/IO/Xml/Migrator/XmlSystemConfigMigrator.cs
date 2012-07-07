using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Vixen.IO.Xml.Migrator {
	class XmlSystemConfigMigrator : IO.Migrator {
		private MigrationSegment[] _segments;

		public XmlSystemConfigMigrator() {
			_segments = new[] {
				new MigrationSegment(1, 2, _Version_1_to_2),
				new MigrationSegment(2, 3, _Version_2_to_3),
				new MigrationSegment(3, 4, _Version_3_to_4),
				new MigrationSegment(4, 5, _Version_4_to_5),
				new MigrationSegment(5, 6, _Version_5_to_6),
				new MigrationSegment(6, 7, _Version_6_to_7),
				new MigrationSegment(7, 8, _Version_7_to_8),
				new MigrationSegment(8, 9, _Version_8_to_9),
				new MigrationSegment(9, 10, _Version_9_to_10),
				new MigrationSegment(10, 11, _Version_10_to_11)};
		}

		override public IEnumerable<MigrationSegment> ValidMigrations {
			get { return _segments; }
		}

		private void _Version_1_to_2(object content) {
			XElement contentElement = (XElement)content;
			contentElement.Add(new XElement("Identity", Guid.NewGuid().ToString()));
		}

		private void _Version_2_to_3(object content) {
			XElement contentElement = (XElement)content;
			contentElement.Add(new XElement("Controllers"));
		}

		private void _Version_3_to_4(object content) {
			//XElement controllersElement = _content.Element(XmlControllerCollectionSerializer.ELEMENT_CONTROLLERS);
			//if(controllersElement != null) {
			//    foreach(XElement controllerElement in controllersElement.Elements(XmlControllerCollectionSerializer.ELEMENT_CONTROLLER)) {
			//        XElement transformDataElement = controllerElement.Element(ELEMENT_TRANSFORM_DATA);
			//        string content = transformDataElement.InnerXml();
			//        transformDataElement.Remove();
			//        XElement moduleDataElement = new XElement(ELEMENT_MODULE_DATA, XElement.Parse(content));
			//        controllerElement.Add(moduleDataElement);
			//    }
			//}
			//return element;
		}

		private void _Version_4_to_5(object content) {
			XElement contentElement = (XElement)content;
			contentElement.Add(new XElement("DisabledControllers"));
		}

		private void _Version_5_to_6(object content) {
			XElement contentElement = (XElement)content;
			contentElement.Add(new XElement("ControllerLinks"));
		}

		private void _Version_6_to_7(object content) {
			XElement contentElement = (XElement)content;
			XElement controllersElement = contentElement.Element("Controllers");
			if(controllersElement != null) {
				foreach(XElement controllerElement in controllersElement.Elements("Controller")) {
					XElement outputsElement = controllerElement.Element("Outputs");
					if(outputsElement != null) {
						foreach(XElement outputElement in outputsElement.Elements("Output")) {
							outputElement.Add(new XElement("FilterNodes"));
						}
					}
				}
			}
		}

		private void _Version_7_to_8(object content) {
			XElement contentElement = (XElement)content;
			contentElement.Add(new XElement("Previews"));
		}

		private void _Version_8_to_9(object content) {
			XElement contentElement = (XElement)content;
			contentElement.Add(new XElement("AllowFilterEvaluation", true));
		}

		private void _Version_9_to_10(object content) {
			XElement contentElement = (XElement)content;
			contentElement.Add(new XElement("AllowSubordinateEffects", true));
		}

		private void _Version_10_to_11(object content) {
			XElement contentElement = (XElement)content;
			contentElement.Add(new XElement("SmartControllers"));
		}
	}
}
