using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Vixen.Sys;

namespace Vixen.IO.Xml {
	class XmlProgramWriter : XmlWriterBase<Program> {
		private const string ELEMENT_ROOT = "Program";
		private const string ELEMENT_SEQUENCES = "Sequences";
		private const string ELEMENT_SEQUENCE = "Sequence";
		private const string ATTR_FILE_NAME = "fileName";

		override protected XElement _CreateContent(Program program) {
			return new XElement(ELEMENT_ROOT,
				new XElement(ELEMENT_SEQUENCES,
					program.Sequences.Select(x =>
						new XElement(ELEMENT_SEQUENCE,
							new XAttribute(ATTR_FILE_NAME, x.Name)))));
		}
	}
}
