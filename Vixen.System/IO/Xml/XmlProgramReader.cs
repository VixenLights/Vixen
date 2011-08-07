using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Vixen.Sys;
using Vixen.Common;

namespace Vixen.IO.Xml {
	class XmlProgramReader : IReader {
		private const string ELEMENT_ROOT = "Program";
		private const string ELEMENT_SEQUENCES = "Sequences";
		private const string ELEMENT_SEQUENCE = "Sequence";
		private const string ATTR_FILE_NAME = "fileName";

		public object Read(string filePath) {
			XElement element = Helper.LoadXml(filePath);
			Program program = CreateObject(element, filePath);
			return program;
		}

		public Program CreateObject(XElement element, string filePath) {
			string name = System.IO.Path.GetFileNameWithoutExtension(filePath);
	
			Program program = new Program(name);

			foreach(string sequenceFileName in element.Element(ELEMENT_SEQUENCES).Elements(ELEMENT_SEQUENCE)) {
				ISequence sequence = Sequence.Load(sequenceFileName);
				program.Add(sequence);
			}

			return program;
		}
	}
}
