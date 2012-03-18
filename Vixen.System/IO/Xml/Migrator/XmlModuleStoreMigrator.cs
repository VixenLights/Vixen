using System.Xml.Linq;

namespace Vixen.IO.Xml.Migrator {
	class XmlModuleStoreMigrator : EmptyMigrator {
		private XElement _content;

		public XmlModuleStoreMigrator(XElement content) {
			_content = content;
		}
	}
}
