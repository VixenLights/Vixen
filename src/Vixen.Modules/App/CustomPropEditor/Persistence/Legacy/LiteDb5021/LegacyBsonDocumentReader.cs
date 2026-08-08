using System.Buffers.Binary;
using System.IO;
using System.Text;

namespace VixenModules.App.CustomPropEditor.Persistence.Legacy.LiteDb5021;

/// <summary>
/// Reads the subset of BSON values used by LiteDB v4 file records without resolving CLR types.
/// </summary>
internal static class LegacyBsonDocumentReader
{
	public static IReadOnlyDictionary<string, object> Read(byte[] document)
	{
		ArgumentNullException.ThrowIfNull(document);

		var position = 0;
		var value = ReadDocument(document, ref position);
		if (position != document.Length)
		{
			throw new InvalidDataException("The BSON document contains trailing data.");
		}

		return value;
	}

	private static Dictionary<string, object> ReadDocument(ReadOnlySpan<byte> source, ref int position)
	{
		var length = ReadInt32(source, ref position);
		if (length < 5)
		{
			throw new InvalidDataException("The BSON document length is invalid.");
		}

		var end = checked(position + length - sizeof(int));
		if (end > source.Length)
		{
			throw new InvalidDataException("The BSON document extends beyond its source data.");
		}

		var values = new Dictionary<string, object>(StringComparer.Ordinal);
		while (position < end - 1)
		{
			var type = ReadByte(source, ref position);
			var name = ReadCString(source, ref position);
			if (string.Equals(name, "_type", StringComparison.Ordinal))
			{
				throw new InvalidDataException("Legacy prop data contains forbidden _type metadata.");
			}

			values.Add(name, ReadValue(type, source, ref position));
		}

		if (position != end - 1 || ReadByte(source, ref position) != 0)
		{
			throw new InvalidDataException("The BSON document terminator is invalid.");
		}

		return values;
	}

	private static object ReadValue(byte type, ReadOnlySpan<byte> source, ref int position) => type switch
	{
		0x01 => BitConverter.Int64BitsToDouble(ReadInt64(source, ref position)),
		0x02 => ReadString(source, ref position),
		0x03 => ReadDocument(source, ref position),
		0x04 => ReadArray(source, ref position),
		0x05 => ReadBinary(source, ref position),
		0x07 => ReadBytes(source, ref position, 12),
		0x08 => ReadBoolean(source, ref position),
		0x09 => ReadInt64(source, ref position),
		0x0A => null,
		0x10 => ReadInt32(source, ref position),
		0x12 => ReadInt64(source, ref position),
		0x13 => ReadBytes(source, ref position, 16),
		0x7F or 0xFF => null,
		_ => throw new InvalidDataException($"Unsupported BSON type 0x{type:X2}.")
	};

	private static List<object> ReadArray(ReadOnlySpan<byte> source, ref int position)
	{
		var document = ReadDocument(source, ref position);
		return document.OrderBy(pair => int.Parse(pair.Key, System.Globalization.CultureInfo.InvariantCulture))
			.Select(pair => pair.Value)
			.ToList();
	}

	private static byte[] ReadBinary(ReadOnlySpan<byte> source, ref int position)
	{
		var length = ReadInt32(source, ref position);
		_ = ReadByte(source, ref position);
		return ReadBytes(source, ref position, length);
	}

	private static string ReadString(ReadOnlySpan<byte> source, ref int position)
	{
		var length = ReadInt32(source, ref position);
		if (length <= 0)
		{
			throw new InvalidDataException("The BSON string length is invalid.");
		}

		var bytes = ReadBytes(source, ref position, length);
		if (bytes[^1] != 0)
		{
			throw new InvalidDataException("The BSON string terminator is invalid.");
		}

		return Encoding.UTF8.GetString(bytes, 0, bytes.Length - 1);
	}

	private static string ReadCString(ReadOnlySpan<byte> source, ref int position)
	{
		var start = position;
		while (position < source.Length && source[position] != 0)
		{
			position++;
		}

		if (position == source.Length)
		{
			throw new InvalidDataException("The BSON field name terminator is missing.");
		}

		var value = Encoding.UTF8.GetString(source[start..position]);
		position++;
		return value;
	}

	private static byte[] ReadBytes(ReadOnlySpan<byte> source, ref int position, int count)
	{
		if (count < 0 || count > source.Length - position)
		{
			throw new InvalidDataException("The BSON value extends beyond its source data.");
		}

		var value = source.Slice(position, count).ToArray();
		position += count;
		return value;
	}

	private static byte ReadByte(ReadOnlySpan<byte> source, ref int position)
	{
		if (position >= source.Length)
		{
			throw new InvalidDataException("The BSON value is truncated.");
		}

		return source[position++];
	}

	private static bool ReadBoolean(ReadOnlySpan<byte> source, ref int position) => ReadByte(source, ref position) switch
	{
		0 => false,
		1 => true,
		_ => throw new InvalidDataException("The BSON Boolean value is invalid.")
	};

	private static int ReadInt32(ReadOnlySpan<byte> source, ref int position)
	{
		const int size = sizeof(int);
		if (source.Length - position < size)
		{
			throw new InvalidDataException("The BSON Int32 value is truncated.");
		}

		var value = BinaryPrimitives.ReadInt32LittleEndian(source[position..]);
		position += size;
		return value;
	}

	private static long ReadInt64(ReadOnlySpan<byte> source, ref int position)
	{
		const int size = sizeof(long);
		if (source.Length - position < size)
		{
			throw new InvalidDataException("The BSON Int64 value is truncated.");
		}

		var value = BinaryPrimitives.ReadInt64LittleEndian(source[position..]);
		position += size;
		return value;
	}
}
