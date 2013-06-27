using System;
using Vixen.IO.Factory;

namespace Vixen.IO.Persistor
{
	using Vixen.Sys;

	internal class SystemConfigPersistor : IObjectPersistor<SystemConfig>
	{
		public void SaveToFile(SystemConfig obj, string filePath)
		{
			IFileWriter fileWriter = FileWriterFactory.CreateFileWriter();
			IObjectContentReader contentReader = ObjectContentReaderFactory.Instance.CreateSystemConfigContentReader();
			ObjectPersistorService.Instance.SaveToFile(obj, fileWriter, contentReader, filePath);

			if (obj != null) obj.LoadedFilePath = filePath;
		}

		public void SaveToFile(object obj, string filePath)
		{
			if (!(obj is SystemConfig)) throw new InvalidOperationException("Object must be a ModuleStore.");
			SaveToFile((SystemConfig) obj, filePath);
		}
	}
}