using System.IO;
using System.Linq;
using System.Xml.Linq;
using Vixen.Script;
using Vixen.Sys;

namespace Vixen.IO.Xml {
	class XmlScriptSourceFileCollectionSerializer : IXmlSerializer<SourceFileCollection> {
		private const string ELEMENT_SOURCE_FILES = "SourceFiles";
		private const string ELEMENT_SOURCE_FILE = "SourceFile";
		private const string ATTR_DIRECTORY = "directory";
		private const string ATTR_NAME = "name";

		public XElement WriteObject(SourceFileCollection value) {
			// Write the source files and their references.
			return new XElement(ELEMENT_SOURCE_FILES, 
				new XAttribute(ATTR_DIRECTORY, value.Directory),
				value.Files.Select(x => 
				new XElement(ELEMENT_SOURCE_FILE,
					new XAttribute(ATTR_NAME, x.Name)
			)));
		}

		public SourceFileCollection ReadObject(XElement element) {
			SourceFileCollection sourceFileCollection = new SourceFileCollection();
			
			element = element.Element(ELEMENT_SOURCE_FILES);
			if(element != null) {
				XAttribute directoryAttr = element.Attribute(ATTR_DIRECTORY);
				if(directoryAttr != null) {
					sourceFileCollection.Directory = directoryAttr.Value;

					foreach(XElement sourceFileElement in element.Elements(ELEMENT_SOURCE_FILE)) {
						XAttribute fileNameAttr = sourceFileElement.Attribute(ATTR_NAME);
						if(fileNameAttr == null) continue;

						string filePath = Path.Combine(sourceFileCollection.Directory, fileNameAttr.Value);
						SourceFile sourceFile = SourceFile.Load(filePath);
						sourceFileCollection.Files.Add(sourceFile);
					}
				}
			}

			return sourceFileCollection;
		}
	}
}
