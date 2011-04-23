using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Vixen.Common;
using Vixen.Hardware;

namespace Vixen.IO {
	class OutputControllerDefinitionReader : IReader {
		public object Read(string filePath) {
			XElement element = Helper.LoadXml(filePath);
			return OutputControllerDefinition.ReadXml(element);
		}
	}
}
