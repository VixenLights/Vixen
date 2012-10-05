using Vixen.IO.Xml;

namespace Vixen.IO.Factory {
	class FileReaderFactory : IFileReaderFactory {
		static private FileReaderFactory _instance;

		private FileReaderFactory() {
		}

		public static FileReaderFactory Instance {
			get { return _instance ?? (_instance = new FileReaderFactory()); }
		}

		public IFileReader CreateFileReader() {
			return new XElementFileReader();
		}
	}
}
