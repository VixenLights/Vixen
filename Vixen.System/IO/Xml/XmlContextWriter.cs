using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;
using System.IO.Compression;
using Vixen.Sys;

namespace Vixen.IO.Xml {
	class XmlContextWriter : XmlWriterBase<SystemContext> {
		private const string ELEMENT_ROOT = "SystemContext";
		private const string ELEMENT_SOURCE_IDENTITY = "SourceIdentity";
		private const string ELEMENT_CONTEXT_NAME = "Name";
		private const string ELEMENT_CONTEXT_DESCRIPTION = "Description";
		private const string ELEMENT_FILES = "Files";
		private const string ELEMENT_FILE = "File";
		private const string ATTRIBUTE_FILE_PATH = "path";

		protected override XElement _CreateContent(SystemContext obj) {
			return new XElement(ELEMENT_ROOT,
				_WriteSourceIdentity(obj),
				_WriteContextName(obj),
				_WriteContextDescription(obj),
				_WriteFiles(obj));
		}

		private XElement _WriteSourceIdentity(SystemContext context) {
			return new XElement(ELEMENT_SOURCE_IDENTITY, context.SourceIdentity.ToString());
		}

		private XElement _WriteContextName(SystemContext context) {
			return new XElement(ELEMENT_CONTEXT_NAME, context.ContextName);
		}

		private XElement _WriteContextDescription(SystemContext context) {
			return new XElement(ELEMENT_CONTEXT_DESCRIPTION, context.ContextDescription);
		}

		private XElement _WriteFiles(SystemContext context) {
			return new XElement(ELEMENT_FILES,
				context.Select(x => new XElement(ELEMENT_FILE,
					new XAttribute(ATTRIBUTE_FILE_PATH, x.FilePath),
					Convert.ToBase64String(_Compress(x.FileContent)))));
		}

		private byte[] _Compress(byte[] fileBytes) {
			byte[] bytes = null;

			using(MemoryStream fileStream = new MemoryStream(fileBytes)) {
				using(MemoryStream compressedStream = new MemoryStream()) {
					using(GZipStream zipStream = new GZipStream(compressedStream, CompressionMode.Compress)) {
						int value;
						while((value = fileStream.ReadByte()) != -1) {
							zipStream.WriteByte((byte)value);
						}
						zipStream.Close();
						bytes = compressedStream.ToArray();
					}
				}
			}

			return bytes;
		}
	}
}
