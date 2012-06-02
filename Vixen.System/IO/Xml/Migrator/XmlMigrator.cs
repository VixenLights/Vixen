using System.Collections.Generic;
using System.Xml.Linq;

namespace Vixen.IO.Xml.Migrator {
	abstract class XmlMigrator : IO.Migrator {
		protected XmlMigrator(XElement content) {
			Content = content;
		}

		protected XElement Content { get; private set; }

		public abstract override IEnumerable<MigrationSegment> ValidMigrations { get; }
	}
}
