using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;
using Vixen.Sys;
using Vixen.Common;
using Vixen.Module.Effect;

namespace Vixen.IO.Xml {
	class XmlScriptSequenceWriter : XmlSequenceWriter {
		protected override XElement _WriteContent(Sequence sequence) {
			if(!(sequence is ScriptSequence)) throw new InvalidOperationException("Attempt to serialize a " + sequence.GetType().ToString() + " as a ScriptSequence.");
			ScriptSequence scriptSequence = (ScriptSequence)sequence;
			return new XElement("Script",
				_WriteLanguage(scriptSequence),
				_WriteClassName(scriptSequence),
				_WriteSourceFiles(scriptSequence),
				_WriteFrameworkAssemblies(scriptSequence),
				_WriteExternalAssemblies(scriptSequence));
		}

		private XElement _WriteLanguage(ScriptSequence sequence) {
			return new XElement("Language", sequence.Language);
		}

		private XElement _WriteClassName(ScriptSequence sequence) {
			return new XElement("ClassName", sequence.ClassName);
		}

		private XElement _WriteSourceFiles(ScriptSequence sequence) {
			// Make sure source directory exists.
			string sourcePath = Path.Combine(sequence.SourceDirectory, sequence.Name);
			Helper.EnsureDirectory(sourcePath);

			// Write the source files and their references.
			return new XElement("SourceFiles", sequence.SourceFiles.Select(x => {
				x.Save(sourcePath);
				return new XElement("SourceFile",
					new XAttribute("name", x.Name));
			}));
		}

		private XElement _WriteFrameworkAssemblies(ScriptSequence sequence) {
			return new XElement("FrameworkAssemblies", sequence.FrameworkAssemblies.Select(x =>
				new XElement("Assembly",
					new XAttribute("name", x))));
		}

		private XElement _WriteExternalAssemblies(ScriptSequence sequence) {
			return new XElement("ExternalAssemblies", sequence.ExternalAssemblies.Select(x =>
				new XElement("Assembly",
					new XAttribute("name", x))));
		}
	}
}
