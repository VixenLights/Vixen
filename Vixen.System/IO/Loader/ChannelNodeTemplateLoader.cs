using Vixen.IO.Factory;

namespace Vixen.IO.Loader {
	using Vixen.Sys;

	class ChannelNodeTemplateLoader : IObjectLoader<ChannelNodeTemplate> {
		public ChannelNodeTemplate LoadFromFile(string filePath) {
			IFileReader fileReader = FileReaderFactory.Instance.CreateFileReader();
			IObjectContentWriter contentWriter = ObjectContentWriterFactory.Instance.CreateChannelNodeTemplateContentWriter();
			IContentMigrator contentMigrator = ContentMigratorFactory.Instance.CreateChannelNodeTemplateContentMigrator();
			ChannelNodeTemplate obj = MigratingObjectLoaderService.Instance.LoadFromFile<ChannelNodeTemplate>(filePath, fileReader, contentWriter, contentMigrator, ObjectVersion.ChannelNodeTemplate);
			return obj;
		}

		object IObjectLoader.LoadFromFile(string filePath) {
			return LoadFromFile(filePath);
		}
	}
}
