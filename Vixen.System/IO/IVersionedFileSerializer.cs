namespace Vixen.IO {
	public interface IVersionedFileSerializer : IFileSerializer {
		//int FileVersion { get; set; }
		int FileVersion { get; }
		int ClassVersion { get; }
	}
}
