using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Vixen.Sys;

namespace Vixen.IO.Xml {
	class XmlProgramReader : XmlReaderBase<Program> {
		private const string ELEMENT_ROOT = "Program";
		private const string ELEMENT_SEQUENCES = "Sequences";
		private const string ELEMENT_SEQUENCE = "Sequence";
		private const string ATTR_FILE_NAME = "fileName";

		override protected Program _CreateObject(XElement element, string filePath) {
			string name = System.IO.Path.GetFileNameWithoutExtension(filePath);
	
			Program program = new Program(name);
			return program;
		}

		protected override void _PopulateObject(Program obj, XElement element) {
			foreach(string sequenceFileName in element.Element(ELEMENT_SEQUENCES).Elements(ELEMENT_SEQUENCE)) {
				ISequence sequence = Sequence.Load(sequenceFileName);
				obj.Add(sequence);
			}
		}

		protected override IEnumerable<Func<XElement, XElement>> _ProvideMigrations(int versionAt, int targetVersion) {
			return new Func<XElement, XElement>[] { };
		}
	}
}
