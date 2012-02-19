using System;

namespace Vixen.IO {
	class MigrationSegment : IEquatable<MigrationSegment> {
		public MigrationSegment(int fromVersion, int toVersion) {
			FromVersion = fromVersion;
			ToVersion = toVersion;
		}

		public int FromVersion { get; private set; }
		public int ToVersion { get; private set; }

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
