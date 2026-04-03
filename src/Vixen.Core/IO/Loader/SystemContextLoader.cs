using Vixen.IO.Factory;
using Vixen.Sys;

namespace Vixen.IO.Loader
{
	internal class SystemContextLoader : IObjectLoader<SystemContext>
	{
		public SystemContext LoadFromFile(string filePath)
		{
			IFileReader fileReader = FileReaderFactory.Instance.CreateFileReader();
			IObjectContentWriter contentWriter = ObjectContentWriterFactory.Instance.CreateSystemContextContentWriter();
			IContentMigrator contentMigrator = ContentMigratorFactory.Instance.CreateSystemContextContentMigrator();
			SystemContext obj = MigratingObjectLoaderService.Instance.LoadFromFile<SystemContext>(filePath, fileReader,
			                                                                                      contentWriter, contentMigrator,
			                                                                                      ObjectVersion.SystemContext);
			return obj;
		}

		object IObjectLoader.LoadFromFile(string filePath)
		{
			return LoadFromFile(filePath);
		}
	}
}