namespace Vixen.IO.Factory
{
	internal interface IFileReaderFactory
	{
		IFileReader CreateFileReader();
		IFileReader CreateBinaryFileReader();
	}
}