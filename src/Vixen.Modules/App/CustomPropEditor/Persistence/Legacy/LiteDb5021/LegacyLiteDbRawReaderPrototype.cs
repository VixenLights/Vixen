using System.Buffers.Binary;
using System.IO;
using System.Text;

namespace VixenModules.App.CustomPropEditor.Persistence.Legacy.LiteDb5021;

/// <summary>
/// Proves that unencrypted LiteDB v4 Custom Prop data can be read as raw BSON without opening or upgrading a database.
/// </summary>
internal sealed class LegacyLiteDbRawReaderPrototype
{
	private const int PageSize = 4096;
	private const int PageHeaderLength = 25;
	private const int MaximumVisitedPages = 1_000_000;
	private const string HeaderInfo = "** This is a LiteDB file **";

	public LegacyLiteDbRawDocument Read(string path)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(path);

		using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, PageSize, FileOptions.SequentialScan);
		var reader = new V4FileReader(stream);
		var propDocuments = reader.ReadCollection("props");
		if (propDocuments.Count != 1)
		{
			throw new InvalidDataException("A legacy Custom Prop file must contain exactly one props document.");
		}

		_ = LegacyBsonDocumentReader.Read(propDocuments[0]);
		var image = ReadImage(reader);
		return new LegacyLiteDbRawDocument(propDocuments[0], image.Bytes, image.EntryName);
	}

	public static bool IsV4Header(ReadOnlySpan<byte> header) => header.Length >= 53 &&
		Encoding.UTF8.GetString(header.Slice(25, HeaderInfo.Length)) == HeaderInfo &&
		header[52] == 7;

	private static (byte[] Bytes, string EntryName) ReadImage(V4FileReader reader)
	{
		var files = reader.ReadCollection("_files");
		var file = files
			.Select(LegacyBsonDocumentReader.Read)
			.FirstOrDefault(document => document.TryGetValue("_id", out var id) && id is string value &&
				(value == "$/image/background.jpg" || value == "$/image/background.png"));

		if (file is null || file["_id"] is not string fileId)
		{
			throw new InvalidDataException("The legacy Custom Prop background image is missing.");
		}

		var chunks = reader.ReadCollection("_chunks")
			.Select(LegacyBsonDocumentReader.Read)
			.Select(document => (Document: document, Chunk: GetChunkNumber(document, fileId)))
			.Where(value => value.Chunk is not null)
			.OrderBy(value => value.Chunk)
			.ToList();

		if (chunks.Count == 0 || chunks.Any(value => value.Document.GetValueOrDefault("data") is not byte[]))
		{
			throw new InvalidDataException("The legacy Custom Prop background image chunks are invalid.");
		}

		using var output = new MemoryStream();
		foreach (var chunk in chunks)
		{
			output.Write((byte[])chunk.Document["data"]!);
		}

		return (output.ToArray(), Path.GetFileName(fileId));
	}

	private static int? GetChunkNumber(IReadOnlyDictionary<string, object> document, string fileId)
	{
		if (document.GetValueOrDefault("_id") is not string id || !id.StartsWith(fileId + "\\", StringComparison.Ordinal))
		{
			return null;
		}

		return int.TryParse(id[(fileId.Length + 1)..], out var chunk) ? chunk : null;
	}

	private sealed class V4FileReader(Stream stream)
	{
		private readonly Dictionary<string, uint> _collections = ReadHeader(stream);

		public List<byte[]> ReadCollection(string collection)
		{
			if (!_collections.TryGetValue(collection, out var collectionPageId))
			{
				throw new InvalidDataException($"The legacy file does not contain the {collection} collection.");
			}

			var collectionPage = ReadPage(collectionPageId);
			EnsurePageType(collectionPage, 2, collection);
			var position = PageHeaderLength;
			_ = ReadString(collectionPage, ref position);
			position += 12;
			var indexHeadPageId = ReadIndexHeadPageId(collectionPage, ref position);
			var documents = new List<byte[]>();
			foreach (var indexPageId in VisitIndexPages(indexHeadPageId))
			{
				var indexPage = ReadPage(indexPageId);
				EnsurePageType(indexPage, 3, collection);
				var indexPosition = PageHeaderLength;
				var itemCount = ReadUInt16(indexPage, 13);
				for (var i = 0; i < itemCount; i++)
				{
					var block = ReadIndexNode(indexPage, ref indexPosition);
					if (block.PageId != uint.MaxValue)
					{
						documents.Add(ReadDataBlock(block));
					}
				}
			}

			return documents;
		}

		private static Dictionary<string, uint> ReadHeader(Stream stream)
		{
			var page = ReadPage(stream, 0);
			if (!IsV4Header(page))
			{
				throw new InvalidDataException("The file is not a supported LiteDB v4 data file.");
			}

			if (page.AsSpan(85, 16).IndexOfAnyExcept((byte)0) >= 0)
			{
				throw new InvalidDataException("Encrypted LiteDB v4 Custom Prop files are not supported by this proof of concept.");
			}

			var position = 101;
			var count = ReadByte(page, ref position);
			var collections = new Dictionary<string, uint>(count, StringComparer.Ordinal);
			for (var i = 0; i < count; i++)
			{
				collections.Add(ReadString(page, ref position), ReadUInt32(page, ref position));
			}

			return collections;
		}

		private uint ReadIndexHeadPageId(byte[] collectionPage, ref int position)
		{
			for (var i = 0; i < 16; i++)
			{
				var field = ReadString(collectionPage, ref position);
				position += 1;
				var headPageId = ReadUInt32(collectionPage, ref position);
				position += 12;
				if (i == 0 && field.Length > 0)
				{
					return headPageId;
				}
			}

			throw new InvalidDataException("The legacy props collection has no primary index.");
		}

		private IEnumerable<uint> VisitIndexPages(uint startPageId)
		{
			var pending = new Stack<uint>();
			var visited = new HashSet<uint>();
			pending.Push(startPageId);
			while (pending.Count > 0)
			{
				if (visited.Count >= MaximumVisitedPages)
				{
					throw new InvalidDataException("The legacy index page limit was exceeded.");
				}

				var pageId = pending.Pop();
				if (!visited.Add(pageId))
				{
					continue;
				}

				var page = ReadPage(pageId);
				EnsurePageType(page, 3, "index");
				var position = PageHeaderLength;
				var itemCount = ReadUInt16(page, 13);
				for (var i = 0; i < itemCount; i++)
				{
					var block = ReadIndexNode(page, ref position);
					if (block.PreviousPageId != uint.MaxValue && !visited.Contains(block.PreviousPageId))
					{
						pending.Push(block.PreviousPageId);
					}
					if (block.NextPageId != uint.MaxValue && !visited.Contains(block.NextPageId))
					{
						pending.Push(block.NextPageId);
					}
				}

				yield return pageId;
			}
		}

		private static DataBlockAddress ReadIndexNode(byte[] page, ref int position)
		{
			_ = ReadUInt16(page, ref position);
			var levels = ReadByte(page, ref position);
			position += 13;
			var keyLength = ReadUInt16(page, ref position);
			position += 1 + keyLength;
			var pageId = ReadUInt32(page, ref position);
			var index = ReadUInt16(page, ref position);
			var previousPageId = ReadUInt32(page, ref position);
			position += 2;
			var nextPageId = ReadUInt32(page, ref position);
			position += 2;
			position += checked((levels - 1) * 12);
			return new DataBlockAddress(pageId, index, previousPageId, nextPageId);
		}

		private byte[] ReadDataBlock(DataBlockAddress address)
		{
			var page = ReadPage(address.PageId);
			EnsurePageType(page, 4, "data");
			var position = PageHeaderLength;
			var itemCount = ReadUInt16(page, 13);
			for (var i = 0; i < itemCount; i++)
			{
				var index = ReadUInt16(page, ref position);
				var extendPageId = ReadUInt32(page, ref position);
				var length = ReadUInt16(page, ref position);
				var data = ReadBytes(page, ref position, length);
				if (index == address.Index)
				{
					return extendPageId == uint.MaxValue ? data : ReadExtendData(extendPageId);
				}
			}

			throw new InvalidDataException("The legacy data block is missing.");
		}

		private byte[] ReadExtendData(uint pageId)
		{
			using var output = new MemoryStream();
			var visited = new HashSet<uint>();
			while (pageId != uint.MaxValue)
			{
				if (!visited.Add(pageId) || visited.Count > MaximumVisitedPages)
				{
					throw new InvalidDataException("The legacy extend page chain is invalid.");
				}

				var page = ReadPage(pageId);
				EnsurePageType(page, 5, "extend");
				var length = ReadUInt16(page, 13);
				output.Write(page, PageHeaderLength, length);
				pageId = ReadUInt32(page, 9);
			}

			return output.ToArray();
		}

		private byte[] ReadPage(uint pageId) => ReadPage(stream, pageId);

		private static byte[] ReadPage(Stream stream, uint pageId)
		{
			var offset = checked((long)pageId * PageSize);
			if (offset > stream.Length - PageSize)
			{
				throw new InvalidDataException("The legacy page points outside the data file.");
			}

			stream.Position = offset;
			var page = new byte[PageSize];
			stream.ReadExactly(page);
			return page;
		}

		private static void EnsurePageType(byte[] page, byte expected, string purpose)
		{
			if (page[4] != expected)
			{
				throw new InvalidDataException($"The legacy {purpose} page has an unexpected type.");
			}
		}

		private static byte ReadByte(byte[] source, ref int position) => source[position++];
		private static ushort ReadUInt16(byte[] source, int position) => BinaryPrimitives.ReadUInt16LittleEndian(source.AsSpan(position));
		private static ushort ReadUInt16(byte[] source, ref int position) { var value = ReadUInt16(source, position); position += sizeof(ushort); return value; }
		private static uint ReadUInt32(byte[] source, int position) => BinaryPrimitives.ReadUInt32LittleEndian(source.AsSpan(position));
		private static uint ReadUInt32(byte[] source, ref int position) { var value = BinaryPrimitives.ReadUInt32LittleEndian(source.AsSpan(position)); position += sizeof(uint); return value; }
		private static string ReadString(byte[] source, ref int position) { var length = checked((int)ReadUInt32(source, ref position)); var value = Encoding.UTF8.GetString(source, position, length); position += length; return value; }
		private static byte[] ReadBytes(byte[] source, ref int position, int count) { var value = source.AsSpan(position, count).ToArray(); position += count; return value; }
		private readonly record struct DataBlockAddress(uint PageId, ushort Index, uint PreviousPageId, uint NextPageId);
	}
}
