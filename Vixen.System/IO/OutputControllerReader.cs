using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Vixen.Hardware;
using Vixen.Common;

namespace Vixen.IO {
	class OutputControllerReader : IReader {
		public object Read(string filePath) {
			// This is just a pass-through now.
			// Keeping it around so that serialization to/from a file can be done predictably.
			XElement element = Helper.LoadXml(filePath);
			return OutputController.ReadXml(element);
		}
	}
}
