using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;
using Vixen.Sys;
using Vixen.Script;

namespace Vixen.IO.Xml {
	class XmlScriptSequenceReader : XmlSequenceReader {
		protected override void _ReadContent(XElement element, Sequence sequence) {
			ScriptSequence scriptSequence = (ScriptSequence)sequence;
			element = element.Element("Script");
			_ReadLanguage(element, scriptSequence);
			_ReadClassName(element, scriptSequence);
			_ReadSourceFiles(element, scriptSequence);
			_ReadFrameworkAssemblies(element, scriptSequence);
			_ReadExternalAssemblies(element, scriptSequence);
		}

		private void _ReadLanguage(XElement element, ScriptSequence scriptSequence) {
			scriptSequence.Language = element.Element("Language").Value;
		}

		private void _ReadClassName(XElement element, ScriptSequence scriptSequence) {
			scriptSequence.ClassName = element.Element("ClassName").Value;
		}

		private void _ReadSourceFiles(XElement element, ScriptSequence scriptSequence) {
			string sourcePath = Path.Combine(scriptSequence.SourceDirectory, scriptSequence.Name);
			scriptSequence.SourceFiles.Clear();
			IEnumerable<string> fileNames = element
				.Element("SourceFiles")
				.Elements("SourceFile")
				.Select(x => x.Attribute("name").Value);
			foreach(string fileName in fileNames) {
				string filePath = Path.Combine(sourcePath, fileName);
				scriptSequence.SourceFiles.Add(SourceFile.Load(filePath));
			}
		}

		private void _ReadFrameworkAssemblies(XElement element, ScriptSequence scriptSequence) {
			scriptSequence.FrameworkAssemblies = new HashSet<string>(
				element
					.Element("FrameworkAssemblies")
					.Elements("Assembly")
					.Select(x => x.Attribute("name").Value));
		}

		private void _ReadExternalAssemblies(XElement element, ScriptSequence scriptSequence) {
			scriptSequence.ExternalAssemblies = new HashSet<string>(
				element
					.Element("ExternalAssemblies")
					.Elements("Assembly")
					.Select(x => x.Attribute("name").Value));
		}
	}
}
