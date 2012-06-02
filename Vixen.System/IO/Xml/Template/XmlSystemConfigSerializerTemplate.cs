using System.Xml.Linq;
using Vixen.IO.Policy;
using Vixen.IO.Xml.Migrator;
using Vixen.IO.Xml.Policy;
using Vixen.Sys;

namespace Vixen.IO.Xml.Template {
	class XmlSystemConfigSerializerTemplate : IXmlStandardFileWriteTemplate<SystemConfig>, IXmlStandardFileReadTemplate<SystemConfig> {
		public XmlVersionedContent GetContentNode() {
			return new XmlVersionedContent("SystemConfig");
		}

		public IFilePolicy GetEmptyFilePolicy() {
			return new XmlSystemConfigFilePolicy();
		}

		public IFilePolicy GetFilePolicy(SystemConfig obj, XElement content) {
			return new XmlSystemConfigFilePolicy(obj, content);
		}

		public string GetAbsoluteFilePath(string filePath) {
			return filePath;
		}


		public SystemConfig CreateNewObjectFor(string filePath) {
			return new SystemConfig();
		}

		public IMigrator GetMigrator(XElement content) {
			return new XmlSystemConfigMigrator(content);
		}
	}
}
