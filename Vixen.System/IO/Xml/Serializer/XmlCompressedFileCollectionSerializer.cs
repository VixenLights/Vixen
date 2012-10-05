using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Vixen.Sys;

namespace Vixen.IO.Xml.Serializer {
	class XmlCompressedFileCollectionSerializer : IXmlSerializer<IEnumerable<IPackageFileContent>> {
		private const string ELEMENT_FILES = "Files";

		public XElement WriteObject(IEnumerable<IPackageFileContent> value) {
			XmlCompressedFileSerializer compressedFileSerializer = new XmlCompressedFileSerializer();
			return new XElement(ELEMENT_FILES, value.Select(compressedFileSerializer.WriteObject));
		}

		public IEnumerable<IPackageFileContent> ReadObject(XElement element) {
			List<IPackageFileContent> files = new List<IPackageFileContent>();

			XElement filesElement = element.Element(ELEMENT_FILES);
			if(filesElement != null) {
				XmlCompressedFileSerializer compressedFileSerializer = new XmlCompressedFileSerializer();
				files.AddRange(filesElement.Elements().Select(compressedFileSerializer.ReadObject).NotNull());
			}

			return files;
		}
	}
}
