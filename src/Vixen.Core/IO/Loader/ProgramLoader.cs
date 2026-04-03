using Vixen.IO.Factory;
using Vixen.Sys;

namespace Vixen.IO.Loader
{
	internal class ProgramLoader : IObjectLoader<Program>
	{
		public Program LoadFromFile(string filePath)
		{
			IFileReader fileReader = FileReaderFactory.Instance.CreateFileReader();
			IObjectContentWriter contentWriter = ObjectContentWriterFactory.Instance.CreateProgramContentWriter();
			IContentMigrator contentMigrator = ContentMigratorFactory.Instance.CreateProgramContentMigrator();
			Program obj = MigratingObjectLoaderService.Instance.LoadFromFile<Program>(filePath, fileReader, contentWriter,
			                                                                          contentMigrator, ObjectVersion.Program);

			if (obj != null) obj.FilePath = filePath;

			return obj;
		}

		object IObjectLoader.LoadFromFile(string filePath)
		{
			return LoadFromFile(filePath);
		}
	}
}