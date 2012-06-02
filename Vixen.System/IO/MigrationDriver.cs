using System.Collections.Generic;
using System.Linq;
using Vixen.IO.Result;
using Vixen.Sys;

namespace Vixen.IO {
	class MigrationDriver {
		private int _fromVersion;
		private int _toVersion;
		private IMigrator _migrator;

		public MigrationDriver(int fromVersion, int toVersion, IMigrator migrator) {
			_fromVersion = fromVersion;
			_toVersion = toVersion;
			_migrator = migrator;
		}

		public bool MigrationNeeded {
			get { return _fromVersion != _toVersion; }
		}

		public bool MigrationPathAvailable {
			get {
				MigrationPath migrationPath = _BuildPath(_fromVersion, _toVersion);
				return migrationPath != null;
			}
		}

		public IEnumerable<IFileOperationResult> Migrate() {
			List<IFileOperationResult> results = new List<IFileOperationResult>();

			MigrationPath migrationPath = _BuildPath(_fromVersion, _toVersion);
			if(migrationPath != null) {
				foreach(MigrationSegment migrationSegment in migrationPath) {
					results.AddRange(_migrator.Migrate(migrationSegment.FromVersion, migrationSegment.ToVersion));
				}
				results.Add(new MigrationResult(true, "Migration successful.", _fromVersion, _toVersion));
			} else {
				results.Add(new MigrationResult(false, "No migration path available.", _fromVersion, _toVersion));
			}

			return results;
		}

		private MigrationPath _BuildPath(int fromVersion, int toVersion) {
			return _BuildPathUsingSequentialVersions(fromVersion, toVersion);
		}

		private MigrationPath _BuildPathUsingSequentialVersions(int fromVersion, int toVersion) {
			MigrationPath migrationPath = new MigrationPath();
			
			while(fromVersion != toVersion) {
				MigrationSegment segment = _FindExactMigration(fromVersion, fromVersion+1);
				if(segment == null) return null;
				migrationPath.Add(segment);
				fromVersion++;
			}
			
			return migrationPath;
		}

		private MigrationSegment _FindExactMigration(int fromVersion, int toVersion) {
			return _migrator.ValidMigrations.FirstOrDefault(x => x.FromVersion == fromVersion && x.ToVersion == toVersion);
		}

		private MigrationPath _BuildPathUsingBestFit(int fromVersion, int toVersion) {
			MigrationPath migrationPath = new MigrationPath();
			
			while(fromVersion != toVersion) {
				IEnumerable<MigrationSegment> segments = _FindMigrationsGoingFrom(fromVersion, toVersion);
				if(!segments.Any()) return null;
				
				segments = _OrderByBestFit(segments);
				MigrationSegment bestFitSegment = segments.First();
				migrationPath.Add(bestFitSegment);

				if(_SegmentIsCompleting(bestFitSegment, fromVersion, toVersion)) {
					break;
				}

				fromVersion = bestFitSegment.ToVersion;
			}
			
			return migrationPath;
		}

		private IEnumerable<MigrationSegment> _OrderByBestFit(IEnumerable<MigrationSegment> segments) {
			return segments.OrderByDescending(x => x.ToVersion);
		}

		private bool _SegmentIsCompleting(MigrationSegment migrationSegment, int fromVersion, int toVersion) {
			return migrationSegment.FromVersion == fromVersion && migrationSegment.ToVersion == toVersion;
		}

		private IEnumerable<MigrationSegment> _FindMigrationsGoingFrom(int fromVersion, int toVersion) {
			return _migrator.ValidMigrations.Where(x => x.FromVersion == fromVersion && x.ToVersion <= toVersion);
		}
	}
}
