using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Vixen.Sys;

namespace Vixen.IO.Xml {
	class XmlProgramWriter : IWriter {
		private const string ELEMENT_ROOT = "Program";
		private const string ELEMENT_SEQUENCES = "Sequences";
		private const string ELEMENT_SEQUENCE = "Sequence";
		private const string ATTR_FILE_NAME = "fileName";

		public void Write(string filePath, object value) {
			if(!(value is Program)) throw new InvalidOperationException("Attempt to serialize a " + value.GetType().ToString() + " as a program.");
			
			Program program = (Program)value;
			XElement doc = CreateContent(program);
			doc.Save(filePath);
		}

		public XElement CreateContent(Program program) {
			return new XElement(ELEMENT_ROOT,
				new XElement(ELEMENT_SEQUENCES,
					program.Sequences.Select(x =>
						new XElement(ELEMENT_SEQUENCE,
							new XAttribute(ATTR_FILE_NAME, x.Name)))));
		}
	}
}
