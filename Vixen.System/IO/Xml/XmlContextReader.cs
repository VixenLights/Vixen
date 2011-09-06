using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;
using System.IO.Compression;
using Vixen.Sys;

namespace Vixen.IO.Xml {
	class XmlContextReader : XmlReaderBase<SystemContext> {
		private const string ELEMENT_ROOT = "SystemContext";
		private const string ELEMENT_SOURCE_IDENTITY = "SourceIdentity";
		private const string ELEMENT_CONTEXT_NAME = "Name";
		private const string ELEMENT_CONTEXT_DESCRIPTION = "Description";
		private const string ELEMENT_FILES = "Files";
		private const string ELEMENT_FILE = "File";
		private const string ATTRIBUTE_FILE_PATH = "path";
		
		protected override SystemContext _CreateObject(XElement element, string filePath) {
			Guid sourceIdentity = _ReadSourceIdentity(element);
			SystemContext context = new SystemContext(sourceIdentity);
			return context;
		}

		protected override void _PopulateObject(SystemContext obj, XElement element) {
			string contextName = _ReadContextName(element);
			string contextDescription = _ReadContextDescription(element);
			IPackageFileContent[] files = _ReadFiles(element).ToArray();

			obj.ContextName = contextName;
			obj.ContextDescription = contextDescription;
			foreach(IPackageFileContent file in files) {
				obj.AddFile(file);
			}
		}

		private Guid _ReadSourceIdentity(XElement element) {
			Guid sourceIdentity = Guid.Parse(element.Element(ELEMENT_SOURCE_IDENTITY).Value);
			return sourceIdentity;
		}

		private string _ReadContextName(XElement element) {
			return element.Element(ELEMENT_CONTEXT_NAME).Value;
		}

		private string _ReadContextDescription(XElement element) {
			return element.Element(ELEMENT_CONTEXT_DESCRIPTION).Value;
		}

		private IEnumerable<IPackageFileContent> _ReadFiles(XElement element) {
			return element.Element(ELEMENT_FILES).Elements(ELEMENT_FILE).Select(x =>
				new ExistingContextFile(x.Attribute(ATTRIBUTE_FILE_PATH).Value, Convert.FromBase64String(x.Value)));
		}

		#region ExistingContextFile
		class ExistingContextFile : IPackageFileContent {
			private byte[] _compressedFileContent;
			private byte[] _decompressedFileContent;

			public ExistingContextFile(string destinationFilePath, byte[] compressedFileContent) {
				_compressedFileContent = compressedFileContent;
				FilePath = destinationFilePath;
			}

			public byte[] FileContent {
				get {
					if(_decompressedFileContent == null) {
						_decompressedFileContent = _Decompress(_compressedFileContent);
					}
					return _decompressedFileContent;
				}
			}

			public string FilePath { get; private set; }

			static private byte[] _Decompress(byte[] fileBytes) {
				byte[] bytes = null;

				using(MemoryStream fileStream = new MemoryStream(fileBytes)) {
					using(GZipStream zipStream = new GZipStream(fileStream, CompressionMode.Decompress)) {
						using(MemoryStream decompressedStream = new MemoryStream()) {
							int value;
							while((value = zipStream.ReadByte()) != -1) {
								decompressedStream.WriteByte((byte)value);
							}
							zipStream.Close();
							bytes = decompressedStream.ToArray();
						}
					}
				}

				return bytes;
			}
		}
		#endregion
	}
}
