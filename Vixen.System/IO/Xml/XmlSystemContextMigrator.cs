using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Vixen.IO.Xml {
	class XmlSystemContextMigrator : EmptyMigrator {
		private XElement _content;

		public XmlSystemContextMigrator(XElement content) {
			_content = content;
		}
}
}
