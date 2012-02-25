using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Vixen.Script;
using Vixen.Sys;

namespace Vixen.IO.Xml {
	class XmlScriptSourceFileCollectionSerializer : IXmlSerializer<IEnumerable<SourceFile>> {
		private const string ELEMENT_SOURCE_FILES = "SourceFiles";
		private const string ELEMENT_SOURCE_FILE = "SourceFile";
		private const string ATTR_NAME = "name";

		public XElement WriteObject(IEnumerable<SourceFile> value) {
			// Write the source files and their references.
			return new XElement(ELEMENT_SOURCE_FILES,
				value.Select(x =>
				new XElement(ELEMENT_SOURCE_FILE,
					new XAttribute(ATTR_NAME, x.Name)
			)));
		}

		public IEnumerable<SourceFile> ReadObject(XElement element) {
			List<SourceFile> sourceFiles = new List<SourceFile>();

			element = element.Element(ELEMENT_SOURCE_FILES);
			if(element != null) {
				foreach(XElement sourceFileElement in element.Elements(ELEMENT_SOURCE_FILE)) {
					string fileName = XmlHelper.GetAttribute(sourceFileElement, ATTR_NAME);
					if(fileName == null) continue;

					//string filePath = Path.Combine(sourceFileCollection.Directory, fileNameAttr.Value);
					//SourceFile sourceFile = SourceFile.Load(filePath);
					SourceFile sourceFile = new SourceFile(fileName);
					sourceFiles.Add(sourceFile);
				}
			}

			return sourceFiles;
		}
	}
}
