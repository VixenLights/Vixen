using Vixen.Sys;

namespace Vixen.IO {
	interface IFileSerializer<T, U> 
		where T : class
		where U : class, IFileOperationResult {
		U Read(string filePath);
		U Write(T value, string filePath);
	}
}
