namespace Vixen.IO {
	interface IFileWriter<in T> : IFileWriter
		where T : class {
		void WriteFile(string filePath, T content);
	}

	interface IFileWriter {
		void WriteFile(string filePath, object content);
	}
}
