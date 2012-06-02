using System.IO;
using System.Xml.Linq;
using Vixen.Sys;

namespace Vixen.IO.Xml {
	class XmlFileLoader : IFileLoader<XElement> {
		public XElement Load(string filePath) {
			using(FileStream fileStream = new FileStream(filePath, FileMode.Open)) {
				using(StreamReader reader = new StreamReader(fileStream)) {
					return XElement.Load(reader);
				}
			}
		}
	}
}
