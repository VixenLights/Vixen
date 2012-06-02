using System;
using System.Collections.Generic;
using System.Linq;
using Vixen.IO.Result;
using Vixen.Sys;

namespace Vixen.IO {
	abstract class Migrator : IMigrator {
		public IEnumerable<IFileOperationResult> Migrate(int fromVersion, int toVersion) {
			MigrationResult migrationResult;

			MigrationSegment migrationSegment = ValidMigrations.FirstOrDefault(x => x.FromVersion == fromVersion && x.ToVersion == toVersion);
			if(migrationSegment != null) {
				Exception exception = _CatchMigrationException(migrationSegment.Execute);
				if(exception == null) {
					migrationResult = new MigrationResult(true, "Migration successful.", fromVersion, toVersion);
				} else {
					migrationResult = new MigrationResult(false, exception.Message, fromVersion, toVersion);
				}
			} else {
				migrationResult = new MigrationResult(false, "No migration path available.", fromVersion, toVersion);
			}

			return migrationResult.AsEnumerable();
		}

		public abstract IEnumerable<MigrationSegment> ValidMigrations { get; }

		private Exception _CatchMigrationException(Action migrationAction) {
			try {
				migrationAction();
				return null;
			} catch(Exception ex) {
				return ex;
			}
		}
	}
}
