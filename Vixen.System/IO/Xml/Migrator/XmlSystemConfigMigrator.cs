using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Vixen.IO.Xml.Migrator {
	class XmlSystemConfigMigrator : XmlMigrator {
		private MigrationSegment[] _segments;

		public XmlSystemConfigMigrator(XElement content)
			: base(content) {
			_segments = new[] {
				new MigrationSegment(1, 2, _Version_1_to_2),
				new MigrationSegment(2, 3, _Version_2_to_3),
				new MigrationSegment(3, 4, _Version_3_to_4),
				new MigrationSegment(4, 5, _Version_4_to_5),
				new MigrationSegment(5, 6, _Version_5_to_6),
				new MigrationSegment(6, 7, _Version_6_to_7),
				new MigrationSegment(7, 8, _Version_7_to_8),
				new MigrationSegment(8, 9, _Version_8_to_9),
				new MigrationSegment(9, 10, _Version_9_to_10)};
		}

		override public IEnumerable<MigrationSegment> ValidMigrations {
			get { return _segments; }
		}

		private void _Version_1_to_2() {
			Content.Add(new XElement("Identity", Guid.NewGuid().ToString()));
		}

		private void _Version_2_to_3() {
			Content.Add(new XElement("Controllers"));
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
			Content.Add(new XElement("DisabledControllers"));
		}

		private void _Version_5_to_6() {
			Content.Add(new XElement("ControllerLinks"));
		}

		private void _Version_6_to_7() {
			XElement controllersElement = Content.Element("Controllers");
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

		private void _Version_7_to_8() {
			Content.Add(new XElement("Previews"));
		}

		private void _Version_8_to_9() {
			Content.Add(new XElement("AllowFilterEvaluation", true));
		}

		private void _Version_9_to_10() {
			Content.Add(new XElement("AllowSubordinateEffects", true));
		}
	}
	//class XmlSystemConfigMigrator : IMigrator {
	//    private XElement _content;
	//    private MigrationSegment[] _segments;

	//    public XmlSystemConfigMigrator(XElement content) {
	//        _content = content;
	//        _segments = new[] {
	//            new MigrationSegment(1, 2, _Version_1_to_2),
	//            new MigrationSegment(2, 3, _Version_2_to_3),
	//            new MigrationSegment(3, 4, _Version_3_to_4),
	//            new MigrationSegment(4, 5, _Version_4_to_5),
	//            new MigrationSegment(5, 6, _Version_5_to_6),
	//            new MigrationSegment(6, 7, _Version_6_to_7),
	//            new MigrationSegment(7, 8, _Version_7_to_8),
	//            new MigrationSegment(8, 9, _Version_8_to_9),
	//            new MigrationSegment(9, 10, _Version_9_to_10)};
	//    }

	//    public IEnumerable<IFileOperationResult> Migrate(int fromVersion, int toVersion) {
	//        //*** this is boilerplate code
	//        MigrationResult migrationResult;

	//        MigrationSegment migrationSegment = _segments.FirstOrDefault(x => x.FromVersion == fromVersion && x.ToVersion == toVersion);
	//        if(migrationSegment != null) {
	//            Exception exception = _CatchMigrationException(migrationSegment.Execute);
	//            if(exception == null) {
	//                migrationResult = new MigrationResult(true, "Migration successful.", fromVersion, toVersion);
	//            } else {
	//                migrationResult = new MigrationResult(false, exception.Message, fromVersion, toVersion);
	//            }
	//        } else {
	//            migrationResult = new MigrationResult(false, "No migration path available.", fromVersion, toVersion);
	//        }

	//        //MigrationResult migrationResult;
	//        //Action migrationAction = null;

	//        //if(fromVersion == 1 && toVersion == 2) migrationAction = _Version_1_to_2;
	//        //else if(fromVersion == 2 && toVersion == 3) migrationAction = _Version_2_to_3;
	//        //else if(fromVersion == 3 && toVersion == 4) migrationAction = _Version_3_to_4;
	//        //else if(fromVersion == 4 && toVersion == 5) migrationAction = _Version_4_to_5;
	//        //else if(fromVersion == 5 && toVersion == 6) migrationAction = _Version_5_to_6;
	//        //else if(fromVersion == 6 && toVersion == 7) migrationAction = _Version_6_to_7;
	//        //else if(fromVersion == 7 && toVersion == 8) migrationAction = _Version_7_to_8;
	//        //else if(fromVersion == 8 && toVersion == 9) migrationAction = _Version_8_to_9;
	//        //else if(fromVersion == 9 && toVersion == 10) migrationAction = _Version_9_to_10;

	//        //if(migrationAction != null) {
	//        //    Exception exception = _CatchMigrationException(migrationAction);
	//        //    if(exception == null) {
	//        //        migrationResult = new MigrationResult(true, "Migration successful.", fromVersion, toVersion);
	//        //    } else {
	//        //        migrationResult = new MigrationResult(false, exception.Message, fromVersion, toVersion);
	//        //    }
	//        //} else {
	//        //    migrationResult = new MigrationResult(false, "No migration path available.", fromVersion, toVersion);
	//        //}

	//        return migrationResult.AsEnumerable();
	//    }

	//    //*** shouldn't the available migration segments be coupled to what actually do the migrations
	//    //    so they the two can't be out of sync?  Maybe a MS also provides the migration?
	//    //-> And the Migrate method would find the migrations among the segments?
	//    public IEnumerable<MigrationSegment> ValidMigrations {
	//        get { return _segments; }
	//        //get { 
	//        //    yield return new MigrationSegment(1, 2);
	//        //    yield return new MigrationSegment(2, 3);
	//        //    yield return new MigrationSegment(3, 4);
	//        //    yield return new MigrationSegment(4, 5);
	//        //    yield return new MigrationSegment(5, 6);
	//        //    yield return new MigrationSegment(6, 7);
	//        //    yield return new MigrationSegment(7, 8);
	//        //    yield return new MigrationSegment(8, 9);
	//        //    yield return new MigrationSegment(9, 10);
	//        //}
	//    }

	//    private Exception _CatchMigrationException(Action migrationAction) {
	//        try {
	//            migrationAction();
	//            return null;
	//        } catch(Exception ex) {
	//            return ex;
	//        }
	//    }

	//    private void _Version_1_to_2() {
	//        _content.Add(new XElement("Identity", Guid.NewGuid().ToString()));
	//    }

	//    private void _Version_2_to_3() {
	//        _content.Add(new XElement("Controllers"));
	//    }

	//    private void _Version_3_to_4() {
	//        //XElement controllersElement = _content.Element(XmlControllerCollectionSerializer.ELEMENT_CONTROLLERS);
	//        //if(controllersElement != null) {
	//        //    foreach(XElement controllerElement in controllersElement.Elements(XmlControllerCollectionSerializer.ELEMENT_CONTROLLER)) {
	//        //        XElement transformDataElement = controllerElement.Element(ELEMENT_TRANSFORM_DATA);
	//        //        string content = transformDataElement.InnerXml();
	//        //        transformDataElement.Remove();
	//        //        XElement moduleDataElement = new XElement(ELEMENT_MODULE_DATA, XElement.Parse(content));
	//        //        controllerElement.Add(moduleDataElement);
	//        //    }
	//        //}
	//        //return element;
	//    }

	//    private void _Version_4_to_5() {
	//        _content.Add(new XElement("DisabledControllers"));
	//    }

	//    private void _Version_5_to_6() {
	//        _content.Add(new XElement("ControllerLinks"));
	//    }

	//    private void _Version_6_to_7() {
	//        XElement controllersElement = _content.Element("Controllers");
	//        if(controllersElement != null) {
	//            foreach(XElement controllerElement in controllersElement.Elements("Controller")) {
	//                XElement outputsElement = controllerElement.Element("Outputs");
	//                if(outputsElement != null) {
	//                    foreach(XElement outputElement in outputsElement.Elements("Output"))
	//                    {
	//                        outputElement.Add(new XElement("FilterNodes"));
	//                    }
	//                }
	//            }
	//        }
	//    }

	//    private void _Version_7_to_8() {
	//        _content.Add(new XElement("Previews"));
	//    }

	//    private void _Version_8_to_9() {
	//        _content.Add(new XElement("AllowFilterEvaluation", true));
	//    }

	//    private void _Version_9_to_10() {
	//        _content.Add(new XElement("AllowSubordinateEffects", true));
	//    }
	//}
}
