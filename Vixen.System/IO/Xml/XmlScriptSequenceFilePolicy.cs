using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Vixen.Script;
using Vixen.Sys;

namespace Vixen.IO.Xml {
	class XmlScriptSequenceFilePolicy : ScriptSequenceFilePolicy {
		private ScriptSequence _sequence;
		private XElement _content;

		private const string ELEMENT_LANGUAGE = "Language";
		private const string ELEMENT_CLASS_NAME = "ClassName";
		private const string ELEMENT_FRAMEWORK_ASMS = "FrameworkAssemblies";
		private const string ELEMENT_ASM = "Assembly";
		private const string ELEMENT_EXTERNAL_ASMS = "ExternalAssemblies";
		private const string ELEMENT_SOURCE_DIR = "SourceFileDirectory";
		private const string ATTR_NAME = "name";
		
		public XmlScriptSequenceFilePolicy() {
		}

		public XmlScriptSequenceFilePolicy(ScriptSequence sequence, XElement content) {
			_sequence = sequence;
			_content = content;
		}

		protected override void WriteLanguage() {
			_content.Add(new XElement(ELEMENT_LANGUAGE, _sequence.Language));
		}

		protected override void WriteClassName() {
			_content.Add(new XElement(ELEMENT_CLASS_NAME, _sequence.ClassName));
		}

		protected override void WriteSourceFileDirectory() {
			_content.Add(new XElement(ELEMENT_SOURCE_DIR, _sequence.SourceFileDirectory));
		}

		protected override void WriteSourceFiles() {
			//// Make sure source directory exists.
			//string sourcePath = Path.Combine(_sequence.SourceFileDirectory, _sequence.Name);
			//Helper.EnsureDirectory(sourcePath);

			//_sequence.SourceFiles.Directory = sourcePath;

			XmlScriptSourceFileCollectionSerializer serializer = new XmlScriptSourceFileCollectionSerializer();
			_content.Add(serializer.WriteObject(_sequence.GetAllSourceFiles()));
		}

		protected override void WriteSourceFileContent() {
			// Make sure source directory exists.
			string sourcePath = Path.Combine(_sequence.SourceFileDirectory, _sequence.Name);
			Helper.EnsureDirectory(sourcePath);

			foreach(SourceFile sourceFile in _sequence.GetAllSourceFiles()) {
				string filePath = Path.Combine(sourcePath, sourceFile.Name);
				File.WriteAllText(filePath, sourceFile.Contents);
			}
		}

		protected override void WriteFrameworkAssemblies() {
			XElement element = new XElement(ELEMENT_FRAMEWORK_ASMS, _sequence.FrameworkAssemblies.Select(x =>
				new XElement(ELEMENT_ASM,
					new XAttribute(ATTR_NAME, x))));
			_content.Add(element);
		}

		protected override void WriteExternalAssemblies() {
			XElement element = new XElement(ELEMENT_EXTERNAL_ASMS, _sequence.ExternalAssemblies.Select(x =>
				new XElement(ELEMENT_ASM,
					new XAttribute(ATTR_NAME, x))));
			_content.Add(element);
		}

		protected override void ReadLanguage() {
			XElement element = _content.Element(ELEMENT_LANGUAGE);
			_sequence.Language = (element != null) ? element.Value : null;
		}

		protected override void ReadClassName() {
			XElement element = _content.Element(ELEMENT_CLASS_NAME);
			_sequence.ClassName = (element != null) ? element.Value : null;
		}

		protected override void ReadSourceFileDirectory() {
			XElement element = _content.Element(ELEMENT_SOURCE_DIR);
			_sequence.SourceFileDirectory = (element != null) ? element.Value : null;
		}

		protected override void ReadSourceFiles() {
			XmlScriptSourceFileCollectionSerializer serializer = new XmlScriptSourceFileCollectionSerializer();
			_sequence.ClearSourceFiles();
			_sequence.AddSourceFiles(serializer.ReadObject(_content));
		}

		protected override void ReadSourceFileContent() {
			foreach(SourceFile sourceFile in _sequence.GetAllSourceFiles()) {
				string filePath = Path.Combine(_sequence.SourceFileDirectory, sourceFile.Name);
				sourceFile.Contents = File.ReadAllText(filePath);
			}
		}

		protected override void ReadFrameworkAssemblies() {
			_sequence.FrameworkAssemblies = new HashSet<string>(_ReadAssemblyList(_content, ELEMENT_FRAMEWORK_ASMS));
		}

		protected override void ReadExternalAssemblies() {
			_sequence.ExternalAssemblies = new HashSet<string>(_ReadAssemblyList(_content, ELEMENT_EXTERNAL_ASMS));
		}

		private IEnumerable<string> _ReadAssemblyList(XElement element, string containerElementName) {
			List<string> assemblyNames = new List<string>();

			element = element.Element(containerElementName);
			if(element != null) {
				foreach(XElement assemblyElement in element.Elements(ELEMENT_ASM)) {
					XAttribute nameAttr = assemblyElement.Attribute(ATTR_NAME);
					if(nameAttr != null) {
						assemblyNames.Add(nameAttr.Value);
					}
				}
			}

			return assemblyNames;
		}
	}
}
