using System;

namespace Vixen.IO {
	public class MigrationSegment<T> : IMigrationSegment<T>
		where T : class {
		private Func<T, T> _migrationAction;

		public MigrationSegment(int fromVersion, int toVersion, Func<T, T> migrationAction) {
			if(migrationAction == null) throw new ArgumentNullException("migrationAction");

			FromVersion = fromVersion;
			ToVersion = toVersion;
			_migrationAction = migrationAction;
		}

		public int FromVersion { get; private set; }
		public int ToVersion { get; private set; }

		public T Execute(T fileContent) {
			return _migrationAction(fileContent);
		}

		object IMigrationSegment.Execute(object content) {
			if(!(content is T)) throw new InvalidOperationException("Content must be of type " + typeof(T));
			return Execute(content as T);
		}

		public bool Equals(MigrationSegment<T> other) {
			return Equals((IMigrationSegment)other);
		}

		public override bool Equals(object obj) {
			if(ReferenceEquals(null, obj)) return false;
			if(!(obj is IMigrationSegment)) return false;
			return Equals((IMigrationSegment)obj);
		}

		public bool Equals(IMigrationSegment other) {
			return other.FromVersion == FromVersion && other.ToVersion == ToVersion;
		}

		public bool Equals(IMigrationSegment<T> other) {
			return Equals((IMigrationSegment)other);
		}

		public override int GetHashCode() {
			unchecked {
				return (FromVersion*397) ^ ToVersion;
			}
		}
	}
}
