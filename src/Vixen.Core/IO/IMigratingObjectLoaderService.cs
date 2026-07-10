namespace Vixen.IO
{
	internal interface IMigratingObjectLoaderService
	{
		T LoadFromFile<T>(string filePath, IFileReader fileReader, IObjectContentWriter contentWriter,
		                  IContentMigrator contentMigrator, int currentObjectVersion) where T : class, new();

		T LoadFromFile<T>(T objectToPopulate, string filePath, IFileReader fileReader, IObjectContentWriter contentWriter,
		                  IContentMigrator contentMigrator, int currentObjectVersion) where T : class;
	}
}