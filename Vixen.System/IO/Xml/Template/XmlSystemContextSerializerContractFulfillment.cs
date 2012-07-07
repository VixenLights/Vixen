using System.Xml.Linq;
using Vixen.IO.Policy;
//using Vixen.IO.Xml.Migrator;
using Vixen.IO.Xml.Policy;
using Vixen.Sys;

namespace Vixen.IO.Xml.Template {
	class XmlSystemContextSerializerContractFulfillment : IXmlStandardFileWriteTemplate<SystemContext>, IXmlStandardFileReadTemplate<SystemContext> {
		public XmlVersionedContent GetContentNode() {
			return new XmlVersionedContent("SystemContext");
		}

		public IFilePolicy GetEmptyFilePolicy() {
			return new XmlSystemContextFilePolicy();
		}

		public IFilePolicy GetFilePolicy(SystemContext obj, XElement content) {
			return new XmlSystemContextFilePolicy(obj, content);
		}

		public string GetAbsoluteFilePath(string filePath) {
			return filePath;
		}


		public SystemContext CreateNewObjectFor(string filePath) {
			return new SystemContext();
		}

		//public IMigrator GetMigrator(XElement content) {
		//    return new XmlSystemContextMigrator(content);
		//}
	}
}
