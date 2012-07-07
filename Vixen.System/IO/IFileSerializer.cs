//using Vixen.IO.Result;

namespace Vixen.IO {
	public interface IFileSerializer {
		object Read(string filePath);
		void Write(object value, string filePath);
	}

	//interface IFileSerializer<in T, out U> : IFileSerializer
	//    where T : class
	//    where U : class, IFileOperationResult {
	//    U Read(string filePath);
	//    U Write(T value, string filePath);
	//}
}
