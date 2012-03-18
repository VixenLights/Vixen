using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Vixen.IO.Result;
using Vixen.Sys;

namespace Vixen.IO.Xml.Migrator {
	class XmlSystemConfigMigrator : IMigrator {
		private XElement _content;

		public XmlSystemConfigMigrator(XElement content) {
			_content = content;
		}

		public IEnumerable<IFileOperationResult> Migrate(int fromVersion, int toVersion) {
			MigrationResult migrationResult;
			Action migrationAction = null;

			if(fromVersion == 1 && toVersion == 2) migrationAction = _Version_1_to_2;
			else if(fromVersion == 2 && toVersion == 3) migrationAction = _Version_2_to_3;
			else if(fromVersion == 3 && toVersion == 4) migrationAction = _Version_3_to_4;
			else if(fromVersion == 4 && toVersion == 5) migrationAction = _Version_4_to_5;
			else if(fromVersion == 5 && toVersion == 6) migrationAction = _Version_5_to_6;
			else if(fromVersion == 6 && toVersion == 7) migrationAction = _Version_6_to_7;

			if(migrationAction != null) {
				Exception exception = _CatchMigrationException(migrationAction);
				if(exception == null) {
					migrationResult = new MigrationResult(true, "Migration successful.", fromVersion, toVersion);
				} else {
					migrationResult = new MigrationResult(false, exception.Message, fromVersion, toVersion);
				}
			} else {
				migrationResult = new MigrationResult(false, "No migration path available.", fromVersion, toVersion);
			}

			return migrationResult.AsEnumerable();
		}

		public IEnumerable<MigrationSegment> ValidMigrations {
			get { 
				yield return new MigrationSegment(1, 2);
				yield return new MigrationSegment(2, 3);
				yield return new MigrationSegment(3, 4);
				yield return new MigrationSegment(4, 5);
				yield return new MigrationSegment(5, 6);
				yield return new MigrationSegment(6, 7);
			}
		}

		private Exception _CatchMigrationException(Action migrationAction) {
			try {
				migrationAction();
				return null;
			} catch(Exception ex) {
				return ex;
			}
		}

		private void _Version_1_to_2() {
			_content.Add(new XElement("Identity", Guid.NewGuid().ToString()));
		}

		private void _Version_2_to_3() {
			_content.Add(new XElement("Controllers"));
		}

		private void _Version_3_to_4() {
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

		private void _Version_4_to_5() {
			_content.Add(new XElement("DisabledControllers"));
		}

		private void _Version_5_to_6() {
			_content.Add(new XElement("ControllerLinks"));
		}

		private void _Version_6_to_7() {
			XElement controllersElement = _content.Element("Controllers");
			if(controllersElement != null) {
				foreach(XElement controllerElement in controllersElement.Elements("Controller")) {
					XElement outputsElement = controllerElement.Element("Outputs");
					if(outputsElement != null) {
						foreach(XElement outputElement in outputsElement.Elements("Output"))
						{
							outputElement.Add(new XElement("FilterNodes"));
						}
					}
				}
			}
		}
	}
}
