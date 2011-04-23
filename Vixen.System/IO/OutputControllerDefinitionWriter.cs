using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Vixen.Hardware;

namespace Vixen.IO {
	class OutputControllerDefinitionWriter : IWriter {
		public void Write(string filePath, object value) {
			OutputControllerDefinition controllerDefinition = (OutputControllerDefinition)value;
			XElement doc = OutputControllerDefinition.WriteXml(controllerDefinition);
			doc.Save(filePath);
		}
	}
}
