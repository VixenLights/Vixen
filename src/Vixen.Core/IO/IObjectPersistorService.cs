namespace Vixen.IO
{
	internal interface IObjectPersistorService
	{
		void SaveToFile(object obj, IFileWriter fileWriter, IObjectContentReader contentReader, string filePath);
	}
}