using Vixen.IO.Binary;
using Vixen.IO.Xml;

namespace Vixen.IO.Factory
{
	internal class FileWriterFactory
	{
		public static IFileWriter CreateFileWriter()
		{
			return new XElementFileWriter();
		}

		public static IFileWriter CreateBinaryFileWriter()
		{
			return new BinaryFileWriter();
		}
	}
}