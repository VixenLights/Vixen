using System;
using System.Collections.Generic;

namespace Vixen.IO {
	public class EmptyMigrator<T> : IContentMigrator<T>
		where T : class {
		public EmptyMigrator() {
			ValidMigrations = new IMigrationSegment<T>[] { };
		}

		public T MigrateContent(T content, int fromVersion, int toVersion) {
			return content;
		}

		object IContentMigrator.MigrateContent(object content, int fromVersion, int toVersion) {
			if(!(content is T)) throw new InvalidOperationException("Content must be of type " + typeof(T));

			return MigrateContent((T)content, fromVersion, toVersion);
		}

		public IEnumerable<IMigrationSegment<T>> ValidMigrations { get; private set; }

		IEnumerable<IMigrationSegment> IContentMigrator.ValidMigrations {
			get { return ValidMigrations; }
		}
	}
}
