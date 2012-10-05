using System;
using Vixen.IO.Factory;

namespace Vixen.IO.Persistor {
	using Vixen.Sys;

	class ModuleStorePersistor : IObjectPersistor<ModuleStore> {
		public void SaveToFile(ModuleStore obj, string filePath) {
			IFileWriter fileWriter = FileWriterFactory.CreateFileWriter();
			IObjectContentReader contentReader = ObjectContentReaderFactory.Instance.CreateModuleStoreContentReader();
			ObjectPersistorService.Instance.SaveToFile(obj, fileWriter, contentReader, filePath);

			if(obj != null) obj.LoadedFilePath = filePath;
		}

		public void SaveToFile(object obj, string filePath) {
			if(!(obj is ModuleStore)) throw new InvalidOperationException("Object must be a ModuleStore.");
			SaveToFile((ModuleStore)obj, filePath);
		}
	}
}
