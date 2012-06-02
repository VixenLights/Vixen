using System.Xml.Linq;
using Vixen.IO.Policy;
using Vixen.IO.Xml.Migrator;
using Vixen.IO.Xml.Policy;
using Vixen.Sys;

namespace Vixen.IO.Xml.Template {
	class XmlModuleStoreSerializerTemplate : IXmlStandardFileWriteTemplate<ModuleStore>, IXmlStandardFileReadTemplate<ModuleStore> {
		public XmlVersionedContent GetContentNode() {
			return new XmlVersionedContent("ModuleStore");
		}

		public IFilePolicy GetEmptyFilePolicy() {
			return new XmlModuleStoreFilePolicy();
		}

		public IFilePolicy GetFilePolicy(ModuleStore obj, XElement content) {
			return new XmlModuleStoreFilePolicy(obj, content);
		}

		public string GetAbsoluteFilePath(string filePath) {
			return filePath;
		}


		public ModuleStore CreateNewObjectFor(string filePath) {
			return new ModuleStore();
		}

		public IMigrator GetMigrator(XElement content) {
			return new XmlModuleStoreMigrator(content);
		}
	}
}
