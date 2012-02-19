using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Vixen.Sys;

//*** this one actually has migrations
namespace Vixen.IO.Xml {
	class XmlSystemConfigMigrator : IMigrator {
		private XElement _content;

		public XmlSystemConfigMigrator(XElement content) {
			_content = content;
		}

		public IEnumerable<IFileOperationResult> Migrate(int fromVersion, int toVersion) {
			return new FileOperationResult(false, "There is only one version.").AsEnumerable();
		}

		public IEnumerable<MigrationSegment> ValidMigrations {
			get { return Enumerable.Empty<MigrationSegment>(); }
		}
	}
}
