using Vixen.IO.Factory;
using Vixen.Sys;

namespace Vixen.IO.Persistor
{
	internal class ModuleStorePersistor : IObjectPersistor<ModuleStore>
	{
		public void SaveToFile(ModuleStore obj, string filePath)
		{
			IFileWriter fileWriter = FileWriterFactory.CreateFileWriter();
			IObjectContentReader contentReader = ObjectContentReaderFactory.Instance.CreateModuleStoreContentReader();
			ObjectPersistorService.Instance.SaveToFile(obj, fileWriter, contentReader, filePath);

			if (obj != null) obj.LoadedFilePath = filePath;
		}

		public void SaveToFile(object obj, string filePath)
		{
			if (!(obj is ModuleStore)) throw new InvalidOperationException("Object must be a ModuleStore.");
			SaveToFile((ModuleStore) obj, filePath);
		}
	}
}