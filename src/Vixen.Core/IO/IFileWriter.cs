namespace Vixen.IO
{
	internal interface IFileWriter<in T> : IFileWriter
		where T : class
	{
		void WriteFile(string filePath, T content);
	}

	internal interface IFileWriter
	{
		void WriteFile(string filePath, object content);
	}
}