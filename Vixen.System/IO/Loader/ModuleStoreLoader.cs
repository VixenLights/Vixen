using Vixen.IO.Factory;

namespace Vixen.IO.Loader {
	using Vixen.Sys;

	class ModuleStoreLoader : IObjectLoader<ModuleStore> {
		public ModuleStore LoadFromFile(string filePath) {
			IFileReader fileReader = FileReaderFactory.Instance.CreateFileReader();
			IObjectContentWriter contentWriter = ObjectContentWriterFactory.Instance.CreateModuleStoreContentWriter();
			IContentMigrator contentMigrator = ContentMigratorFactory.Instance.CreateModuleStoreContentMigrator();
			ModuleStore obj = MigratingObjectLoaderService.Instance.LoadFromFile<ModuleStore>(filePath, fileReader, contentWriter, contentMigrator, ObjectVersion.ModuleStore);

			if(obj != null) obj.LoadedFilePath = filePath;
	
			return obj;
		}

		object IObjectLoader.LoadFromFile(string filePath) {
			return LoadFromFile(filePath);
		}
	}
}
