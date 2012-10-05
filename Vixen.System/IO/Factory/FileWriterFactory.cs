using Vixen.IO.Xml;

namespace Vixen.IO.Factory {
	class FileWriterFactory {
		static public IFileWriter CreateFileWriter() {
			return new XElementFileWriter();
		}
	}
}
