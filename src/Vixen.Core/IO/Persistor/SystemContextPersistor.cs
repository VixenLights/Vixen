using Vixen.IO.Factory;
using Vixen.Sys;

namespace Vixen.IO.Persistor
{
	internal class SystemContextPersistor : IObjectPersistor<SystemContext>
	{
		public void SaveToFile(SystemContext obj, string filePath)
		{
			IFileWriter fileWriter = FileWriterFactory.CreateFileWriter();
			IObjectContentReader contentReader = ObjectContentReaderFactory.Instance.CreateSystemContextContentReader();
			ObjectPersistorService.Instance.SaveToFile(obj, fileWriter, contentReader, filePath);
		}

		public void SaveToFile(object obj, string filePath)
		{
			if (!(obj is SystemContext)) throw new InvalidOperationException("Object must be a ModuleStore.");
			SaveToFile((SystemContext) obj, filePath);
		}
	}
}