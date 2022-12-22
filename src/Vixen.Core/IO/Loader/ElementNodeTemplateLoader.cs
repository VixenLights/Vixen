using Vixen.IO.Factory;

namespace Vixen.IO.Loader
{
	using Vixen.Sys;

	internal class ElementNodeTemplateLoader : IObjectLoader<ElementNodeTemplate>
	{
		public ElementNodeTemplate LoadFromFile(string filePath)
		{
			IFileReader fileReader = FileReaderFactory.Instance.CreateFileReader();
			IObjectContentWriter contentWriter = ObjectContentWriterFactory.Instance.CreateElementNodeTemplateContentWriter();
			IContentMigrator contentMigrator = ContentMigratorFactory.Instance.CreateElementNodeTemplateContentMigrator();
			ElementNodeTemplate obj = MigratingObjectLoaderService.Instance.LoadFromFile<ElementNodeTemplate>(filePath,
			                                                                                                  fileReader,
			                                                                                                  contentWriter,
			                                                                                                  contentMigrator,
			                                                                                                  ObjectVersion.
			                                                                                                  	ElementNodeTemplate);
			return obj;
		}

		object IObjectLoader.LoadFromFile(string filePath)
		{
			return LoadFromFile(filePath);
		}
	}
}