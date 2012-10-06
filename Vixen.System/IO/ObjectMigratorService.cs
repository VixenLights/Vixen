using System;
using System.IO;
using System.Linq;

namespace Vixen.IO {
	class ObjectMigratorService : IObjectMigratorService {
		static private ObjectMigratorService _instance;

		private ObjectMigratorService() {
		}

		static public ObjectMigratorService Instance {
			get { return _instance ?? (_instance = new ObjectMigratorService()); }
		}

		public object MigrateObject(object content, IContentMigrator migrator, int contentVersion, int targetVersion, string filePath) {
			if(!_MigrationNeeded(contentVersion, targetVersion)) {
				return content;
			}

			if(!_MigrationPathAvailable(migrator, contentVersion, targetVersion)) {
				throw new Exception("The file requires a migration, but a proper migration path is not available.");
			}

			_BackupFile(filePath, contentVersion);

			content = _MigrateContent(content, contentVersion, targetVersion, migrator);

			return content;
		}

		private bool _MigrationNeeded(int contentVersion, int targetVersion) {
			return contentVersion < targetVersion;
		}

		private bool _MigrationPathAvailable(IContentMigrator migrator, int contentVersion, int targetVersion) {
			return _BuildMigrationPath(migrator, contentVersion, targetVersion) != null;
		}

		private MigrationPath _BuildMigrationPath(IContentMigrator migrator, int contentVersion, int targetVersion) {
			return _BuildMigrationPathUsingSequentialVersions(migrator, contentVersion, targetVersion);
		}

		private void _BackupFile(string filePath, int contentVersion) {
			File.Copy(filePath, filePath + "." + contentVersion, true);
		}

		private object _MigrateContent(object content, int contentVersion, int targetVersion, IContentMigrator migrator) {
			MigrationPath migrationPath = _BuildMigrationPath(migrator, contentVersion, targetVersion);

			foreach(IMigrationSegment migrationSegment in migrationPath) {
				try {
					content = migrator.MigrateContent(content, migrationSegment.FromVersion, migrationSegment.ToVersion);
				} catch(Exception ex) {
					throw new Exception("Error when migrating from version " + contentVersion + " to version " + targetVersion, ex);
				}
			}
			
			return content;
		}

		private MigrationPath _BuildMigrationPathUsingSequentialVersions(IContentMigrator migrator, int fromVersion, int toVersion) {
			MigrationPath migrationPath = new MigrationPath();

			while(fromVersion != toVersion) {
				IMigrationSegment segment = _FindExactMigration(migrator, fromVersion, fromVersion + 1);
				if(segment == null) return null;

				migrationPath.Add(segment);
				fromVersion++;
			}

			return migrationPath;
		}

		private IMigrationSegment _FindExactMigration(IContentMigrator migrator, int fromVersion, int toVersion) {
			return migrator.ValidMigrations.FirstOrDefault(x => x.FromVersion == fromVersion && x.ToVersion == toVersion);
		}
	}
}
