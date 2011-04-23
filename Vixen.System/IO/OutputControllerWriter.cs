using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Vixen.Sys;
using Vixen.Hardware;

namespace Vixen.IO {
	class OutputControllerWriter : IWriter {
		public void Write(string filePath, object value) {
			OutputController controller = (OutputController)value;
			// This is just a pass-through now.
			// Keeping it around so that serialization to/from a file can be done predictably.
			XElement doc = OutputController.WriteXml(controller);
			doc.Save(filePath);
		}
	}
}
