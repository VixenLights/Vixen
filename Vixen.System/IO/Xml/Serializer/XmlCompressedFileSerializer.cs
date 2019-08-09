using System;
using System.Xml.Linq;
using Vixen.Sys;

namespace Vixen.IO.Xml.Serializer
{
	internal class XmlCompressedFileSerializer : IXmlSerializer<IPackageFileContent>
	{
		private static NLog.Logger logging = NLog.LogManager.GetCurrentClassLogger();

		private const string ELEMENT_FILE = "File";
		private const string ATTR_FILE_PATH = "path";

		public XElement WriteObject(IPackageFileContent value)
		{
			FileCompressor fileCompressor = new FileCompressor();
			return new XElement(ELEMENT_FILE,
			                    new XAttribute(ATTR_FILE_PATH, value.FilePath),
			                    Convert.ToBase64String(fileCompressor.Compress(value.FileContent)));
		}

		public IPackageFileContent ReadObject(XElement element)
		{
			try {
				string filePath = XmlHelper.GetAttribute(element, ATTR_FILE_PATH);
				if (filePath != null) {
					return new ExistingContextFile(filePath, Convert.FromBase64String(element.Value));
				}
				return null;
			} catch (Exception e) {
				logging.Error(e, "Error loading Compressed File from XML");
				return null;
			}

		}

		#region ExistingContextFile

		private class ExistingContextFile : IPackageFileContent
		{
			private byte[] _compressedFileContent;
			private byte[] _decompressedFileContent;

			public ExistingContextFile(string destinationFilePath, byte[] compressedFileContent)
			{
				_compressedFileContent = compressedFileContent;
				FilePath = destinationFilePath;
			}

			public byte[] FileContent
			{
				get
				{
					if (_decompressedFileContent == null) {
						FileCompressor fileCompressor = new FileCompressor();
						_decompressedFileContent = fileCompressor.Decompress(_compressedFileContent);
					}
					return _decompressedFileContent;
				}
			}

			public string FilePath { get; private set; }
		}

		#endregion
	}
}