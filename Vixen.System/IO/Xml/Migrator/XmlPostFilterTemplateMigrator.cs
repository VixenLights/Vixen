using System.Xml.Linq;

namespace Vixen.IO.Xml.Migrator {
	class XmlPostFilterTemplateMigrator : EmptyMigrator {
		private XElement _content;

		public XmlPostFilterTemplateMigrator(XElement content) {
			_content = content;
		}
	}
}
