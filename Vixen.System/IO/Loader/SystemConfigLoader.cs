using Vixen.IO.Factory;

namespace Vixen.IO.Loader {
	using Vixen.Sys;

	class SystemConfigLoader : IObjectLoader<SystemConfig> {
		public SystemConfig LoadFromFile(string filePath) {
			IFileReader fileReader = FileReaderFactory.Instance.CreateFileReader();
			IObjectContentWriter contentWriter = ObjectContentWriterFactory.Instance.CreateSystemConfigContentWriter();
			IContentMigrator contentMigrator = ContentMigratorFactory.Instance.CreateSystemConfigContentMigrator();
			SystemConfig obj = MigratingObjectLoaderService.Instance.LoadFromFile<SystemConfig>(filePath, fileReader, contentWriter, contentMigrator, ObjectVersion.SystemConfig);

			if(obj != null) obj.LoadedFilePath = filePath;
			
			return obj;
		}

		object IObjectLoader.LoadFromFile(string filePath) {
			return LoadFromFile(filePath);
		}
	}
}
