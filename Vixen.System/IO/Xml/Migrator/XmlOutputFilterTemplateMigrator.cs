using System.Xml.Linq;

namespace Vixen.IO.Xml.Migrator {
	class XmlOutputFilterTemplateMigrator : EmptyMigrator {
		private XElement _content;

		public XmlOutputFilterTemplateMigrator(XElement content) {
			_content = content;
		}
	}
}
