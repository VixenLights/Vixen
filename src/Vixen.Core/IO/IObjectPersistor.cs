namespace Vixen.IO
{
	internal interface IObjectPersistor<in T> : IObjectPersistor
	{
		void SaveToFile(T obj, string filePath);
	}

	internal interface IObjectPersistor
	{
		void SaveToFile(object obj, string filePath);
	}
}