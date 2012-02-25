using System.Xml.Linq;

namespace Vixen.IO.Xml {
	class XmlSequenceMigrator : EmptyMigrator {
		private XElement _content;

		public XmlSequenceMigrator(XElement content) {
			_content = content;
		}
	}
}
