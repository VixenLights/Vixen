using System;

namespace Vixen.IO {
	public class MigrationSegment : IEquatable<MigrationSegment> {
		private Action<object> _migrationAction;

		public MigrationSegment(int fromVersion, int toVersion, Action<object> migrationAction) {
			if(migrationAction == null) throw new ArgumentNullException("migrationAction");

			FromVersion = fromVersion;
			ToVersion = toVersion;
			_migrationAction = migrationAction;
		}

		public int FromVersion { get; private set; }
		public int ToVersion { get; private set; }

		public void Execute(object fileContent) {
			_migrationAction(fileContent);
		}

		public bool Equals(MigrationSegment other) {
			return other.FromVersion == FromVersion && other.ToVersion == ToVersion;
		}

		public override bool Equals(object obj) {
			if(ReferenceEquals(null, obj)) return false;
			if(obj.GetType() != typeof(MigrationSegment)) return false;
			return Equals((MigrationSegment)obj);
		}

		public override int GetHashCode() {
			unchecked {
				return (FromVersion*397) ^ ToVersion;
			}
		}
	}
}
