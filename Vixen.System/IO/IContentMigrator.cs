using System.Collections.Generic;

namespace Vixen.IO
{
	public interface IContentMigrator<T> : IContentMigrator
		where T : class
	{
		T MigrateContent(T content, int fromVersion, int toVersion);
		new IEnumerable<IMigrationSegment<T>> ValidMigrations { get; }
	}

	public interface IContentMigrator
	{
		object MigrateContent(object content, int fromVersion, int toVersion);
		IEnumerable<IMigrationSegment> ValidMigrations { get; }
	}
}