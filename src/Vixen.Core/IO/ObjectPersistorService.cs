namespace Vixen.IO
{
	internal class ObjectPersistorService : IObjectPersistorService
	{
		private static ObjectPersistorService _instance;

		private ObjectPersistorService()
		{
		}

		public static ObjectPersistorService Instance
		{
			get { return _instance ?? (_instance = new ObjectPersistorService()); }
		}

		public void SaveToFile(object obj, IFileWriter fileWriter, IObjectContentReader contentReader, string filePath)
		{
			object content = contentReader.ReadContentFromObject(obj);
			fileWriter.WriteFile(filePath, content);
		}
	}
}