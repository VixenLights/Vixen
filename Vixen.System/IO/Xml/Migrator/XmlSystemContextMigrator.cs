using System.Xml.Linq;

namespace Vixen.IO.Xml.Migrator {
	class XmlSystemContextMigrator : EmptyMigrator {
		private XElement _content;

		public XmlSystemContextMigrator(XElement content) {
			_content = content;
		}
	}
}
