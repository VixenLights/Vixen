using Vixen.IO.Factory;
using Vixen.Sys;

namespace Vixen.IO.Persistor
{
	internal class ElementNodeTemplatePersistor : IObjectPersistor<ElementNodeTemplate>
	{
		public void SaveToFile(ElementNodeTemplate obj, string filePath)
		{
			IFileWriter fileWriter = FileWriterFactory.CreateFileWriter();
			IObjectContentReader contentReader = ObjectContentReaderFactory.Instance.CreateElementNodeTemplateContentReader();
			ObjectPersistorService.Instance.SaveToFile(obj, fileWriter, contentReader, filePath);
		}

		void IObjectPersistor.SaveToFile(object obj, string filePath)
		{
			if (!(obj is ElementNodeTemplate)) throw new InvalidOperationException("Object must be a ElementNodeTemplate.");
			SaveToFile((ElementNodeTemplate) obj, filePath);
		}
	}
}