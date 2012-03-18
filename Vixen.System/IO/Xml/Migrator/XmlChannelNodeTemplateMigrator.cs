using System.Xml.Linq;

namespace Vixen.IO.Xml.Migrator {
	class XmlChannelNodeTemplateMigrator : EmptyMigrator {
		private XElement _content;

		public XmlChannelNodeTemplateMigrator(XElement content) {
			_content = content;
		}
	}
}
