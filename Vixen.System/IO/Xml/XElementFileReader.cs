using System.IO;
using System.Xml.Linq;

namespace Vixen.IO.Xml {
	class XElementFileReader : IFileReader<XElement> {
		public XElement ReadFile(string filePath) {
			if(!File.Exists(filePath)) return null;

			using(FileStream fileStream = new FileStream(filePath, FileMode.Open)) {
				using(StreamReader reader = new StreamReader(fileStream)) {
					return XElement.Load(reader);
				}
			}
		}

		object IFileReader.ReadFile(string filePath) {
			return ReadFile(filePath);
		}
	}
}
