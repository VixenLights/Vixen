using System.Collections.Generic;
using System.IO;
using Vixen.IO.Result;

namespace Vixen.IO {
	class GeneralFileMigrationPolicy {
		public void MatureContent(object value, int fileVersion, int classVersion, IMigrator migrator, string originalFilePath) {
			List<IResult> migrationResults = new List<IResult>();

			MigrationDriver migrationDriver = new MigrationDriver(fileVersion, classVersion, migrator);
			if(migrationDriver.MigrationNeeded) {
				if(migrationDriver.MigrationPathAvailable) {
					_BackupFile(originalFilePath, fileVersion);
					migrationResults.AddRange(migrationDriver.Migrate(value));
				} else {
					migrationResults.Add(new MigrationResult(false, "The file requires a migration, but a proper migration path is not available.", fileVersion, classVersion));
				}
			}

			MigrationResults = migrationResults;
		}

		public IEnumerable<IResult> MigrationResults { get; private set; }

		private void _BackupFile(string originalFilePath, int fileVersion) {
			File.Copy(originalFilePath, originalFilePath + "." + fileVersion, true);
		}
	}
}
