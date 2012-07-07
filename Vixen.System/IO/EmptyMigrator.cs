using System.Collections.Generic;
using System.Linq;
using Vixen.IO.Result;
using Vixen.Sys;

namespace Vixen.IO {
	public class EmptyMigrator : IMigrator {
		public IEnumerable<IResult> Migrate(object value, int fromVersion, int toVersion) {
			return new MigrationResult(false, "There is only one version.", fromVersion, toVersion).AsEnumerable();
		}

		public IEnumerable<MigrationSegment> ValidMigrations {
			get { return Enumerable.Empty<MigrationSegment>(); }
		}
	}
}
