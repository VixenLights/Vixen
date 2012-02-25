using System.Xml.Linq;

namespace Vixen.IO.Xml {
	class XmlProgramMigrator : EmptyMigrator {
		private XElement _content;

		public XmlProgramMigrator(XElement content) {
			_content = content;
		}
	}
}
