namespace Vixen.Sys
{
	public interface IPackageFileContent
	{
		string FilePath { get; }
		byte[] FileContent { get; }
	}
}