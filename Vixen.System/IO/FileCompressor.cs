using System.IO;
using System.IO.Compression;

namespace Vixen.IO
{
	internal class FileCompressor
	{
		public byte[] Compress(byte[] fileBytes)
		{
			byte[] bytes;

			using (MemoryStream fileStream = new MemoryStream(fileBytes)) {
				using (MemoryStream compressedStream = new MemoryStream()) {
					using (GZipStream zipStream = new GZipStream(compressedStream, CompressionMode.Compress)) {
						int value;
						while ((value = fileStream.ReadByte()) != -1) {
							zipStream.WriteByte((byte) value);
						}
						zipStream.Close();
						bytes = compressedStream.ToArray();
					}
				}
			}

			return bytes;
		}

		public byte[] Decompress(byte[] fileBytes)
		{
			byte[] bytes;

			using (MemoryStream fileStream = new MemoryStream(fileBytes)) {
				using (GZipStream zipStream = new GZipStream(fileStream, CompressionMode.Decompress)) {
					using (MemoryStream decompressedStream = new MemoryStream()) {
						int value;
						while ((value = zipStream.ReadByte()) != -1) {
							decompressedStream.WriteByte((byte) value);
						}
						zipStream.Close();
						bytes = decompressedStream.ToArray();
					}
				}
			}

			return bytes;
		}
	}
}