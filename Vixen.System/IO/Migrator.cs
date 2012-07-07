using System;
using System.Collections.Generic;
using System.Linq;
using Vixen.IO.Result;
using Vixen.Sys;

namespace Vixen.IO {
	abstract class Migrator : IMigrator {
		public IEnumerable<IResult> Migrate(object fileContent, int fromVersion, int toVersion) {
			MigrationResult migrationResult;

			MigrationSegment migrationSegment = ValidMigrations.FirstOrDefault(x => x.FromVersion == fromVersion && x.ToVersion == toVersion);
			if(migrationSegment != null) {
				try {
					migrationSegment.Execute(fileContent);
					migrationResult = new MigrationResult(true, "Migration successful.", fromVersion, toVersion);
				} catch(Exception ex) {
					migrationResult = new MigrationResult(false, ex.Message, fromVersion, toVersion);
				}
			} else {
				migrationResult = new MigrationResult(false, "No migration path available.", fromVersion, toVersion);
			}

			return migrationResult.AsEnumerable();
		}

		public abstract IEnumerable<MigrationSegment> ValidMigrations { get; }
	}
}
