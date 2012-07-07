using System.Collections.Generic;
using Vixen.IO.Result;

namespace Vixen.IO {
	public interface IMigrator {
		IEnumerable<IResult> Migrate(object value, int fromVersion, int toVersion);
		IEnumerable<MigrationSegment> ValidMigrations { get; }
	}
}
