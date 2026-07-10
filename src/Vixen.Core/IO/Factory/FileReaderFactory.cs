using Vixen.IO.Binary;
using Vixen.IO.Xml;

namespace Vixen.IO.Factory
{
	internal class FileReaderFactory : IFileReaderFactory
	{
		private static FileReaderFactory _instance;

		private FileReaderFactory()
		{
		}

		public static FileReaderFactory Instance
		{
			get { return _instance ?? (_instance = new FileReaderFactory()); }
		}

		public IFileReader CreateFileReader()
		{
			return new XElementFileReader();
		}

		public IFileReader CreateBinaryFileReader()
		{
			return new BinaryFileReader();
		}
	}
}