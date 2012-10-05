namespace Vixen.IO {
	interface IObjectMigratorService {
		object MigrateObject(object content, IContentMigrator migrator, int contentVersion, int targetVersion, string filePath);
	}
}
