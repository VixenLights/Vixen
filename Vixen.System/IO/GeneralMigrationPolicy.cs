using System.Collections.Generic;
using System.IO;
using Vixen.Sys;

namespace Vixen.IO {
	class GeneralMigrationPolicy {
		private IFilePolicy _filePolicy;
		private IMigrator _migrator;
		public GeneralMigrationPolicy(IFilePolicy filePolicy, IMigrator migrator) {
			_filePolicy = filePolicy;
			_migrator = migrator;
		}

		public void MatureContent(int fileVersion, string originalFilePath) {
			List<IFileOperationResult> migrationResults = new List<IFileOperationResult>();

			int policyVersion = _filePolicy.GetVersion();
			MigrationDriver migrationDriver = new MigrationDriver(fileVersion, policyVersion, _migrator);
			if(migrationDriver.MigrationNeeded) {
				if(migrationDriver.MigrationPathAvailable) {
					_BackupFile(originalFilePath, fileVersion);
					migrationResults.AddRange(migrationDriver.Migrate());
				} else {
					migrationResults.Add(new FileOperationResult(false, "The file requires a migration, but a proper migration path is not available."));
				}
			}

			MigrationResults = migrationResults;
		}

		public IEnumerable<IFileOperationResult> MigrationResults { get; private set; }

		private void _BackupFile(string originalFilePath, int fileVersion) {
			File.Copy(originalFilePath, originalFilePath + "." + fileVersion);
		}
	}
}
