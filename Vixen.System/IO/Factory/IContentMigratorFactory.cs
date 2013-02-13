namespace Vixen.IO.Factory {
	interface IContentMigratorFactory {
		IContentMigrator CreateSystemConfigContentMigrator();
		IContentMigrator CreateModuleStoreContentMigrator();
		IContentMigrator CreateSystemContextContentMigrator();
		IContentMigrator CreateProgramContentMigrator();
		IContentMigrator CreateElementNodeTemplateContentMigrator();
		IContentMigrator CreateSequenceContentMigrator(string filePath);
	}
}
