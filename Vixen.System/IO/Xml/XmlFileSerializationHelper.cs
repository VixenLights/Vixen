using System.Collections.Generic;
using System.Xml.Linq;
using Vixen.Sys;

namespace Vixen.IO.Xml {
	class XmlFileSerializationHelper {
		public IEnumerable<IFileOperationResult> EnsureContentIsUpToDate(XElement content, string originalFilePath, IFilePolicy filePolicy, IMigrator migrator) {
			//XmlFileVersion fileVersioner = new XmlFileVersion();
			//int fileVersion = fileVersioner.GetVersion(content, versionAttributeName);
			XmlVersionedContent versionedContent = new XmlVersionedContent(content);
			GeneralMigrationPolicy migrationPolicy = new GeneralMigrationPolicy(filePolicy, migrator);
			migrationPolicy.MatureContent(versionedContent.Version, originalFilePath);
			return migrationPolicy.MigrationResults;
		}

		//and...?
	}
}
