using System.Collections.Generic;
using Vixen.Sys;

namespace Vixen.IO {
	interface IMigrator {
		IEnumerable<IFileOperationResult> Migrate(int fromVersion, int toVersion);
		IEnumerable<MigrationSegment> ValidMigrations { get; }
	}
}
