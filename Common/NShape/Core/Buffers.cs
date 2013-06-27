/******************************************************************************
  Copyright 2009-2012 dataweb GmbH
  This file is part of the NShape framework.
  NShape is free software: you can redistribute it and/or modify it under the 
  terms of the GNU General Public License as published by the Free Software 
  Foundation, either version 3 of the License, or (at your option) any later 
  version.
  NShape is distributed in the hope that it will be useful, but WITHOUT ANY
  WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR 
  A PARTICULAR PURPOSE.  See the GNU General Public License for more details.
  You should have received a copy of the GNU General Public License along with 
  NShape. If not, see <http://www.gnu.org/licenses/>.
******************************************************************************/

using System;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;


namespace Dataweb.Utilities
{
	/// <summary>
	/// Summary description for Buffers.
	/// </summary>
	internal class Buffers
	{
		/// <summary>
		/// Fills buffer with givven value.
		/// </summary>
		/// <param name="buffer">Buffer to fill</param>
		/// <param name="value">Value to fill with</param>
		/// <param name="index">Index of first buffer entry to fill</param>
		/// <param name="count">Number of buffer entries to fill</param>
		public static void Initialize(byte[] buffer, byte value, int index, int count)
		{
			for (int i = index; i < index + count; ++i)
				buffer[i] = value;
		}


		public static void Copy(byte[] sourceBuffer, int sourceIndex, byte[] destinationBuffer, int destinationIndex, int size)
		{
			Array.Copy(sourceBuffer, sourceIndex, destinationBuffer, destinationIndex, size);
		}


		public static byte GetByte(byte[] buffer, int index)
		{
			return buffer[index];
		}


		public static byte GetByte(byte[] buffer, int offset, ref int index)
		{
			++index;
			return GetByte(buffer, offset + index - 1);
		}


		public static int SetByte(byte[] buffer, int index, byte value)
		{
			buffer[index] = value;
			return 1;
		}


		public static void SetByte(byte[] buffer, int offset, ref int index, byte value)
		{
			index += SetByte(buffer, offset + index, value);
		}


		public static char GetChar8(byte[] buffer, int index)
		{
			return (char) buffer[index];
		}


		public static void SetChar8(byte[] buffer, int index, char value)
		{
			buffer[index] = (byte) (value%256);
		}


		public static short GetInt16(byte[] buffer, int index)
		{
			return (short) (buffer[index + 1] << 0x8 | buffer[index]);
		}


		public static short GetInt16(byte[] buffer, int offset, ref int index)
		{
			index += 2;
			return GetInt16(buffer, index - 2);
		}


		public static void SetInt16(byte[] buffer, int index, short value)
		{
			buffer[index] = (byte) value;
			buffer[index + 1] = (byte) (value >> 8);
		}


		public static void SetInt16(byte[] buffer, int offset, ref int index, short value)
		{
			SetInt16(buffer, offset + index, value);
			index += 2;
		}


		public static ushort GetUInt16(byte[] buffer, int index)
		{
			return (ushort) (buffer[index + 1] << 0x8 | buffer[index]);
		}


		public static ushort GetUInt16(byte[] buffer, int offset, ref int index)
		{
			index += 2;
			return GetUInt16(buffer, index - 2);
		}


		public static void SetUInt16(byte[] buffer, int index, ushort value)
		{
			buffer[index] = (byte) value;
			buffer[index + 1] = (byte) (value >> 8);
			// Array.Copy(BitConverter.GetBytes(value), 0, buffer, index, 2);
		}


		public static int GetInt32(byte[] buffer, int index)
		{
			return buffer[index + 3] << 0x18 | buffer[index + 2] << 0x10 | buffer[index + 1] << 0x8 | buffer[index];
			// return BitConverter.ToInt32(buffer, index);
		}


		public static int GetInt32(byte[] buffer, int offset, ref int index)
		{
			index += 4;
			return GetInt32(buffer, offset + index - 4);
		}


		public static int SetInt32(byte[] buffer, int index, int value)
		{
			buffer[index] = (byte) value;
			buffer[index + 1] = (byte) (value >> 8);
			buffer[index + 2] = (byte) (value >> 0x10);
			buffer[index + 3] = (byte) (value >> 0x18);
			return 4;
		}


		public static void SetInt32(byte[] buffer, int offset, ref int index, int value)
		{
			index += SetInt32(buffer, offset + index, value);
		}


		public static uint GetUInt32(byte[] buffer, int index)
		{
			return (uint) (buffer[index + 3] << 0x18 | buffer[index + 2] << 0x10 | buffer[index + 1] << 0x8 | buffer[index]);
		}


		public static long GetInt64(byte[] buffer, int index)
		{
			return (long) (((ulong) GetInt32(buffer, index + 4) << 0x20) + (uint) GetInt32(buffer, index));
			// return BitConverter.ToInt64(buffer, index);
		}


		public static void SetInt64(byte[] buffer, int index, long value)
		{
			buffer[index] = (byte) value;
			buffer[index + 1] = (byte) (value >> 8);
			buffer[index + 2] = (byte) (value >> 0x10);
			buffer[index + 3] = (byte) (value >> 0x18);
			buffer[index + 4] = (byte) (value >> 0x20);
			buffer[index + 5] = (byte) (value >> 0x28);
			buffer[index + 6] = (byte) (value >> 0x30);
			buffer[index + 7] = (byte) (value >> 0x38);
			// Array.Copy(BitConverter.GetBytes(value), 0, buffer, index, 8);
		}


		public static double GetDouble(byte[] buffer, int index)
		{
			return BitConverter.ToDouble(buffer, index);
		}


		public static void SetDouble(byte[] buffer, int index, double value)
		{
			Array.Copy(BitConverter.GetBytes(value), 0, buffer, index, 8);
		}


		public static Guid GetGuid(byte[] buffer, int index)
		{
			return new Guid(GetInt32(buffer, index), GetInt16(buffer, index + 4), GetInt16(buffer, index + 6),
			                buffer[index + 8], buffer[index + 9], buffer[index + 10], buffer[index + 11],
			                buffer[index + 12], buffer[index + 13], buffer[index + 14], buffer[index + 15]);
		}


		public static void SetGuid(byte[] buffer, int index, Guid guid)
		{
			Array.Copy(guid.ToByteArray(), 0, buffer, index, 16);
		}


		public static void GetBytes(byte[] buffer, int index, byte[] data, int destinationIndex, int length)
		{
			Array.Copy(buffer, index, data, destinationIndex, length);
		}


		public static void SetBytes(byte[] buffer, int index, byte[] data, int sourceIndex, int length)
		{
			Array.Copy(data, sourceIndex, buffer, index, length);
		}


		public static void GetChars(byte[] buffer, int offset, ref int index, char[] data, int destinationIndex, int length)
		{
			Encoding.Unicode.GetChars(buffer, offset + index, 2*length, data, destinationIndex);
			index += 2*length;
		}


		public static void MoveBytes(byte[] buffer, int fromIndex, int toIndex, int length)
		{
			Array.Copy(buffer, fromIndex, buffer, toIndex, length);
		}


		public static string GetBPAnsiString(byte[] buffer, int index)
		{
			byte length = GetByte(buffer, index);
			return Encoding.GetEncoding(tdbCodepage).GetString(buffer, index + 1, length);
		}


		public static void SetBPAnsiString(byte[] buffer, int index, string value, int count)
		{
			if (value.Length < count) count = value.Length;
			buffer[index] = (byte) count;
			Encoding.GetEncoding(tdbCodepage).GetBytes(value, 0, count, buffer, index + 1);
		}


		public static string GetWPAnsiString(byte[] buffer, int index)
		{
			return GetWPAnsiString(buffer, 0, ref index);
		}


		public static string GetWPAnsiString(byte[] buffer, int offset, ref int index)
		{
			int length = GetInt16(buffer, offset, ref index);
			string result = Encoding.GetEncoding(tdbCodepage).GetString(buffer, index, length);
			index += length;
			return result;
		}


		public static int SetWPAnsiString(byte[] buffer, int index, string value, int count)
		{
			if (value.Length < count) count = value.Length;
			SetInt16(buffer, index, (short) count);
			Encoding.GetEncoding(tdbCodepage).GetBytes(value, 0, count, buffer, index + 2);
			return 2 + count;
		}


		public static void SetWPAnsiString(byte[] buffer, int offset, ref int index, string value, int count)
		{
			if (count > value.Length) count = value.Length;
			SetInt16(buffer, offset, ref index, (short) count);
			Encoding.GetEncoding(tdbCodepage).GetBytes(value, 0, count, buffer, offset + index);
			index += count;
		}


		public static string GetWPUTF16String(byte[] buffer, int index)
		{
			int length = GetInt16(buffer, index);
			return Encoding.Unicode.GetString(buffer, index + 2, 2*length);
		}


		public static string GetWPUTF16String(byte[] buffer, int offset, ref int index)
		{
			string result = GetWPUTF16String(buffer, offset + index);
			index += 2 + result.Length*2;
			return result;
		}


		public static int SetWPUTF16String(byte[] buffer, int index, string value, int count)
		{
			if (count > value.Length) count = value.Length;
			SetInt16(buffer, index, (short) count);
			Encoding.Unicode.GetBytes(value, 0, count, buffer, index + 2);
			return 2 + 2*count;
		}


		// Count ist die maximale Anzahl an Zeichen zum hineinschreiben.
		public static void SetWPUTF16String(byte[] buffer, int offset, ref int index, string value, int count)
		{
			if (count > value.Length) count = value.Length;
			SetInt16(buffer, offset, ref index, (short) count);
			Encoding.Unicode.GetBytes(value, 0, count, buffer, offset + index);
			index += 2*count;
		}


		/*public static string GetSZString(byte[] buffer, int index) {
			lock (stringBuilder) {
				stringBuilder.Length = 0;
				char c;
				int l = 0;
				do {
					c = (char)GetByte(buffer, index + l);
					if (c == '\0') break;
					stringBuilder.Append(c);
					++l;
				} while (true);
				return stringBuilder.ToString();
			}
		}*/


		public static string GetSZString(byte[] buffer, int index)
		{
			int count = 0;
			for (int i = index; buffer[i] != 0; ++i) ++count;
			return Encoding.GetEncoding(tdbCodepage).GetString(buffer, index, count);
		}


		public static void SetSZString(byte[] buffer, int index, string value, int count)
		{
			if (count > value.Length) count = value.Length;
			Encoding.GetEncoding(tdbCodepage).GetBytes(value, 0, count, buffer, index);
			buffer[index + count] = 0;
		}


		// Vergleich von hinten nach vorne, weil dann auch little endian Integer-Zahlen korrekt verglichen
		// werden.
		internal static short CompareBytes(byte[] buffer1, int offset1, byte[] buffer2, int offset2, int size)
		{
			short result = 0;
			for (int i = size - 1; i >= 0; --i) {
				if (buffer1[offset1 + i] < buffer2[offset2 + i]) {
					result = -1;
					break;
				}
				else if (buffer1[offset1 + i] > buffer2[offset2 + i]) {
					result = +1;
					break;
				}
			}
			return result;
		}


		internal static void EnsureBufferSize(ref byte[] buffer, int pos, int size)
		{
			if (pos + size > buffer.Length) {
				byte[] newBuffer = new byte[buffer.Length + size];
				buffer.CopyTo(newBuffer, 0);
				buffer = newBuffer;
			}
		}


		public const int tdbCodepage = 1252;
	}


	internal class StringStringIterator : IStringIterator
	{
		public StringStringIterator(string value)
		{
			this.value = value;
		}

		#region IStringIterator Members

		public int Length
		{
			get { return value == null ? 0 : value.Length; }
		}

		public char this[int index]
		{
			get { return value[index]; }
		}

		#endregion

		private string value;
	}


	internal class BufferStringIterator : IStringIterator
	{
		#region IStringIterator Members

		public int Length
		{
			get { return length; }
		}


		public char this[int index]
		{
			get
			{
				char result;
				switch (format) {
					case Format.AnsiBytePrefixed:
					case Format.AnsiWordPrefixed:
						Encoding.GetEncoding(Buffers.tdbCodepage).GetChars(buffer, offset + index, 1, chars, 0);
						result = chars[0];
						break;
					case Format.WideWordPrefixed:
						result = (char) Buffers.GetInt16(buffer, offset + index*2);
						break;
					default:
						Debug.Fail("Unexpected BufferStringFormat in BufferStringIterator.this[].");
						result = '\x0';
						break;
				}
				return result;
			}
		}

		#endregion

		public enum Format
		{
			AnsiBytePrefixed,
			AnsiWordPrefixed,
			WideWordPrefixed
		};


		public BufferStringIterator()
		{
		}


		/// <summary>
		/// Nur für internen Gebrauch, z.B. Debuggen
		/// </summary>
		public string String
		{
			get
			{
				switch (format) {
					case Format.AnsiBytePrefixed:
						return Buffers.GetBPAnsiString(buffer, offset - 1);
					case Format.AnsiWordPrefixed:
						return Buffers.GetWPAnsiString(buffer, offset - 2);
					case Format.WideWordPrefixed:
						return Buffers.GetWPUTF16String(buffer, offset - 2);
					default:
						Debug.Fail("Unexpected BufferStringFormat in BufferStringIterator.String.");
						return null;
				}
			}
		}


		public void Reset(byte[] buffer, int offset, Format format)
		{
			this.buffer = buffer;
			this.offset = offset;
			this.format = format;
			switch (format) {
				case Format.AnsiBytePrefixed:
					length = Buffers.GetByte(buffer, 0, ref this.offset);
					break;
				case Format.AnsiWordPrefixed:
				case Format.WideWordPrefixed:
					length = Buffers.GetUInt16(buffer, 0, ref this.offset);
					break;
				default:
					Debug.Fail("Unexpected format in BufferStringIterator.Reset");
					break;
			}
		}

		private byte[] buffer = null;
		private int offset = -1;
		private Format format;
		private int length = -1;
		private char[] chars = new char[1];
	}

	#region Blob

	/// <summary>
	/// Klasse für Binary Large Objects. Verwaltet einen Byte-Puffer, der auch größer
	/// sein kann als der gerade belegte Bereich.
	/// </summary>
	internal class Blob : IComparable
	{
		// 100 KB als Start-Puffergröße für Blobs
		private const int DefBlobCapacityKB = 100;


		internal Blob()
		{
			Capacity = 1024*DefBlobCapacityKB;
		}


		internal Blob(byte[] data)
		{
			this.data = data;
			length = data.Length;
		}


		/// <summary>
		/// Liefert/setzt das Fassungsvermögen des Blobs in Anzahl Bytes.
		/// </summary>
		internal int Capacity
		{
			get { return data.Length; }
			set { SetCapacity(value); }
		}


		/// <summary>
		/// Liefert eine Referenz auf den Datenpuffer
		/// </summary>
		internal byte[] Data
		{
			get { return data; }
		}


		/// <summary>
		/// Liefert die Anzahl der gültigen Bytes im Blob.
		/// </summary>
		internal int Length
		{
			get { return length; }
		}


		/// <summary>
		/// Liefert das maximal mögliche Fassungsvermögen eines Blobs.
		/// </summary>
		internal int MaxCapacity
		{
			get { return Int32.MaxValue; }
		}


		/// <summary>
		/// Leert das Blob, setzt die Anzahl der gültigen Bytes auf 0.
		/// </summary>
		internal void Clear()
		{
			length = 0;
		}


		/// <summary>
		/// Implementation der IComparable-Schnittstelle.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns>-1 wenn this kleiner, +1 wenn this größer, 0 wenn beide Blobs exakt gleich.</returns>
		public int CompareTo(object obj)
		{
			int result = 0;
			if (!(obj is Blob))
				throw new ArgumentException("Blob cannot compare to another blob: compare-value is not of type Blob", "obj");
			Blob other = obj as Blob;
			if (Length < other.Length) result = -1;
			else if (Length > other.Length) result = +1;
			else {
				for (int i = 0; i < Length; ++i) {
					result = data[i].CompareTo(other.data[i]);
					if (result != 0) break;
				}
			}
			return result;
		}


		/// <summary>
		/// Setzt das Fassungsvermögen des Blobs.
		/// Dieses muss größer oder gleich der Anzahl der gültigen Bytes sein (Datenintegrität). 
		/// </summary>
		/// <param name="capacity"></param>
		internal void SetCapacity(int capacity)
		{
			if (capacity <= 0 || capacity > MaxCapacity)
				throw new ArgumentOutOfRangeException("Capacity",
				                                      "Capacity of a blob must satisfy condition: c e [1, " + MaxCapacity.ToString() +
				                                      "]");
			if (capacity < length)
				throw new ArgumentOutOfRangeException("Capacity",
				                                      "Capacity of a blob must be greater or equal as the current length");
			if (data != null) {
				if (capacity != data.Length) {
					byte[] dataBuffer = data;
					data = new byte[capacity];
					Buffers.Copy(dataBuffer, 0, data, 0, length);
					dataBuffer = null;
				}
			}
			else {
				data = new byte[capacity];
			}
		}

		// SetCapacity


		/// <summary>
		/// Hängt einen Block von Bytes an das Blob an.
		/// </summary>
		/// <param name="buffer"></param>
		/// <param name="bufferOffset"></param>
		/// <param name="size"></param>
		internal void Append(byte[] buffer, int bufferOffset, int size)
		{
			if (bufferOffset + size > buffer.Length)
				throw new ArgumentException("Cannot append data from buffer to blob: buffersize is not sufficient",
				                            "bufferOffset + size");
			int capacityNeeded = length + size;
			if (capacityNeeded > Capacity) {
				// Gleich mehr allokieren, damit beim nächten Aufruf erneute Reallokation weniger warscheinlich
				capacityNeeded += Capacity;
				Capacity = capacityNeeded;
			}
			Buffers.Copy(buffer, bufferOffset, data, length, size);
			length += size;
		}

		// Append


		/// <summary>
		/// Kopiert einen Block von Bytes aus dem Blob in einen Puffer.
		/// </summary>
		internal void CopyTo(byte[] buffer, int bufferOffset, int index, int size)
		{
			if (bufferOffset + size > buffer.Length)
				throw new ArgumentException("Cannot copy data from blob to buffer: buffersize is not sufficient",
				                            "bufferOffset + size");
			if (index + size > Length)
				throw new ArgumentException("Cannot copy data from blob to buffer: blob does not provide sufficient data",
				                            "start + size");
			Buffers.Copy(data, index, buffer, bufferOffset, size);
		}


		private int length = 0;
		private byte[] data = null;
	}

	// Blob

	#endregion

	/// <summary>
	/// Zugriff auf die einzelnen Zeichen eines Strings unabhängig von der Art der
	/// physichen Speicherung.
	/// </summary>
	internal interface IStringIterator
	{
		int Length { get; }
		char this[int index] { get; }
	}


	internal class StringExtensions
	{
		// In 1.1 gibts das bei String noch nicht.
		public static bool IsNullOrEmpty(string s)
		{
			return s == null || s == "";
		}


		static StringExtensions()
		{
			upperMap = new byte[256];
			for (int i = 0; i < upperMap.Length; ++i) upperMap[i] = (byte) i;
			for (byte i = (byte) 'a'; i <= (byte) 'z'; ++i) upperMap[i] = (byte) (i - (byte) 'a' + (byte) 'A');
			upperMap[(byte) 'ä'] = (byte) 'Ä';
			upperMap[(byte) 'ö'] = (byte) 'Ö';
			upperMap[(byte) 'ü'] = (byte) 'Ü';
			//
			lowerMap = new byte[256];
			for (int i = 0; i < lowerMap.Length; ++i) lowerMap[i] = (byte) i;
			for (byte i = (byte) 'A'; i <= (byte) 'Z'; ++i) lowerMap[i] = (byte) (i + (byte) 'a' - (byte) 'A');
			lowerMap[(byte) 'Ä'] = (byte) 'ä';
			lowerMap[(byte) 'Ö'] = (byte) 'ö';
			lowerMap[(byte) 'Ü'] = (byte) 'ü';
			//
			umlautReplacementMap =
				"\x80\x81\x82\x83\x84\x85\x86\x87\x88\x89S\x8bO\x8d\x8e\x8f" +
				"\x90\x91\x92\x93\x94\x95\x96\x97\x98\x99s\x9bo\x9d\x9eY" +
				"\xa0\xa1\xaLine\xa3\xa4\xa5\xa6\xa7\xa8\xa9\xaa\xab\xac\xad\xae\xaf" +
				"\xb0\xb1\xbLine\xb3\xb4\xb5\xb6\xb7\xb8\xb9\xba\xbb\xbc\xbd\xbe\xbf" +
				"AAAAAAACEEEEIIIIDNOOOOO\xd7OUUUUY\xdes" +
				"aaaaaaaceeeeiiiidnooooo\xf7ouuuuy\xfey";
			Debug.Assert(umlautReplacementMap.Length == 128);
		}


		/// <summary>
		/// Vergleicht die beiden Strings nach den klassischen TurboDB String-Regeln.
		/// </summary>
		/// <param name="ident1"></param>
		/// <param name="ident2"></param>
		/// <returns></returns>
		// TODO 2: Vorläufige Implementierung, CompareInfo erstellen
		// Funktioniert erstaunlicherweise auch für 'Ä' und 'ä' etc.
		internal static bool IsTdbIdentifierEqual(string ident1, string ident2)
		{
			return (string.Compare(ident1, ident2, true, CultureInfo.InvariantCulture) == 0);
		}


		internal static char ToUpper(char c)
		{
			return (ushort) c < 255 ? (char) upperMap[(ushort) c] : c;
		}


		internal static char ToLower(char c)
		{
			return (ushort) c < 255 ? (char) lowerMap[(ushort) c] : c;
		}


		internal static string ConvertToUpper(string s)
		{
			StringBuilder result = new StringBuilder(s);
			for (int i = 0; i < result.Length; ++i)
				if ((ushort) result[i] < 256) result[i] = (char) upperMap[(ushort) result[i]];
			return result.ToString();
		}


		internal static string ConvertToLower(string s)
		{
			StringBuilder result = new StringBuilder(s);
			for (int i = 0; i < result.Length; ++i)
				if ((ushort) result[i] < 256) result[i] = (char) lowerMap[(ushort) result[i]];
			return result.ToString();
		}


		internal static int TdbCompareStrings(string s1, string s2, bool caseSensitive)
		{
			return TdbCompareStrings(new StringStringIterator(s1), new StringStringIterator(s2), caseSensitive);
		}


		internal static int TdbCompareStrings(string s1, IStringIterator s2, bool caseSensitive)
		{
			return TdbCompareStrings(new StringStringIterator(s1), s2, caseSensitive);
		}


		internal static int TdbCompareStrings(IStringIterator s1, string s2, bool caseSensitive)
		{
			return TdbCompareStrings(s1, new StringStringIterator(s2), caseSensitive);
		}


		// Strings mit IEnumerable<char> zu vergleichen ist nicht sinnvoll, weil dabei jedesmal ein CharComparer-
		// Objekt erzeugt wird.
		internal static int TdbCompareStrings(IStringIterator s1, IStringIterator s2, bool caseSensitive)
		{
			int result = 0;
			int len1 = s1.Length;
			int len2 = s2.Length;
			bool inUmlaut1 = false;
			bool inUmlaut2 = false;
			int p1 = int.MaxValue;
			int p2 = int.MaxValue;
			char c1 = (char) 0;
			char c2 = (char) 0;
			int i1 = 0, i2 = 0;
			while ((i1 < len1) && (i2 < len2)) {
				// Character von s1 behandeln
				c1 = s1[i1];
				if (c1 == '^') c1 = ' ';
				if (c1 == 'ß') {
					c1 = 's';
					if (inUmlaut1) inUmlaut1 = false;
					else {
						inUmlaut1 = true;
						--i1;
					}
				}
				else if (c1 >= 128 && c1 < 256) {
					c1 = umlautReplacementMap[(ushort) c1 & 0x7f];
					if (p1 == int.MaxValue) p1 = i1;
				}
				if (!caseSensitive && c1 < 256) c1 = (char) upperMap[(byte) c1];
				//
				// Character von s2 behandeln
				c2 = s2[i2];
				if (c2 == '^') c2 = ' ';
				if (c2 == 'ß') {
					c2 = 's';
					if (inUmlaut2) inUmlaut2 = false;
					else {
						inUmlaut2 = true;
						--i2;
					}
				}
				else if (c2 >= 128 && c2 < 256) {
					c2 = umlautReplacementMap[(ushort) c2 & 0x7f];
					if (p2 == int.MaxValue) p2 = i2;
				}
				if (!caseSensitive && c2 < 256) c2 = (char) upperMap[(byte) c2];
				//
				if (p1 == p2) {
					p1 = int.MaxValue;
					p2 = int.MaxValue;
				}
				if (c1 != c2) break;
				//
				++i1;
				++i2;
			}
			if (i1 < len1 && i2 < len2) {
				if (c1 < c2) result = -1;
				else if (c1 > c2) result = +1;
				else if (p1 < p2) result = +1;
				else if (p1 > p2) result = -1;
				else result = 0;
			}
			else if (i1 < len1) {
				// Der linke ist länger
				result = +1;
			}
			else if (i2 < len2) {
				// Der rechte ist länger
				result = -1;
			}
			else if (p1 < p2) result = +1;
			else if (p1 > p2) result = -1;
			else result = 0;
			return result;
		}


		internal static string GetRegExForFilePattern(string pattern)
		{
			return GetRegExForPattern(pattern, '?', '*', (char) 0);
		}


		/// <summary>
		/// Liefert einen regulären Ausdruck für das Datei-Muster.
		/// </summary>
		/// <param name="pattern">Muster mit Jokern wie für Windows Dateinamen und SQL LIKE üblich</param>
		/// <param name="singleJoker">Das Zeichen für den Platzhalter für ein einzelnes Zeichen</param>
		/// <param name="multiJoker">Das Zeichen für den Platzhalter für beliebig viele Zeichen</param>
		/// <param name="escapeChar">Das Zeichen, das als Escape-Zeichen verwendet wird</param>
		/// <returns>Regulärer Ausdruck, den man zum Vergleichen benutzen kann</returns>
		internal static string GetRegExForPattern(string pattern, char singleJoker, char multiJoker, char escapeChar)
		{
			const string specialChars = "(){}[].^+";
			StringBuilder result = new StringBuilder();
			for (int i = 0; i < pattern.Length; ++i) {
				// Escape-Sequenz behandeln
				if (pattern[i] == escapeChar && i < pattern.Length - 1
				    && (pattern[i + 1] == singleJoker || pattern[i + 1] == multiJoker || pattern[i + 1] == escapeChar)) {
					// Auch die Joker und escape-Zeichen könnten regex Sonderzeichen sein
					if (pattern[i + 1] == '$') result.Append('$');
					else if (specialChars.IndexOf(pattern[i + 1]) >= 0) {
						result.Append('\\');
						result.Append(pattern[i + 1]);
					}
					else result.Append(pattern[i + 1]);
					result.Append(pattern[i + 1]);
				}
				else if (pattern[i] == singleJoker)
					result.Append(".");
				else if (pattern[i] == multiJoker)
					result.Append(".*");
				else if (pattern[i] == '$')
					result.Append("$$");
				else if (specialChars.IndexOf(pattern[i]) >= 0) {
					result.Append('\\');
					result.Append(pattern[i]);
				}
				else result.Append(pattern[i]);
			} // für jedes Zeichen
			return result.ToString();
		}

		// GetRegExForPattern


		/// <summary>
		/// Liefert True, wenn str exakt auf regEx passt.
		/// </summary>
		internal static bool IsThoroughRegExMatch(Regex regex, string str)
		{
			Match match = regex.Match(str);
			return match.Success && match.Index == 0 && match.Length == str.Length;
		}


		internal static bool IsThoroughRegExMatch(string pattern, string str)
		{
			Match match = Regex.Match(str, pattern);
			return match.Success && match.Index == 0 && match.Length == str.Length;
		}


		/// <summary>
		/// Holt das nächste abgetrennte Wort aus der Liste.
		/// </summary>
		/// <param name="list">Eine Zeichenkette aus durch <separator> getrennten Wörtern</separator></param>
		/// <param name="position">Position des ersten Zeichens des nächsten Wortes in der Liste</param>
		/// <param name="separator">Trennzeichen</param>
		/// <param name="word">Das gefunden Wort</param>
		/// <returns>True, wenn noch ein Wort gefunden wurde</returns>
		/// <remarks>Wörter der Länge 0 dürfen in der Zeichenkette nicht vorkommen.""</remarks>
		internal static bool SplitSeparatedList(string list, char separator, ref int position, out string word)
		{
			bool result;
			if (position < list.Length) {
				int p = position;
				while (p < list.Length && list[p] != separator)
					++p;
				word = list.Substring(position, p - position);
				position = p + 1;
				result = true;
			}
			else {
				word = null;
				result = false;
			}
			return result;
		}


		/// <summary>
		/// Holt das nächste abgetrennte Wort aus der Liste. Beachtet dabei, dass Title 
		/// in Anführungszeichen nicht getrennt wird.
		/// </summary>
		/// <param name="list">Eine Zeichenkette aus durch <separator> getrennten Wörtern</separator></param>
		/// <param name="position">Position des ersten Zeichens des nächsten Wortes in der Liste</param>
		/// <param name="separator">Trennzeichen</param>
		/// <param name="quote">Anführungszeichen</param>
		/// <param name="word">Das gefunden Wort</param>
		/// <returns>True, wenn noch ein Wort gefunden wurde</returns>
		/// <remarks>Wörter der Länge 0 dürfen in der Zeichenkette nicht vorkommen.""</remarks>
		internal static bool SplitSeparatedList(string list, char separator, char quote, ref int position, out string word)
		{
			bool result;
			bool inQuote = false;
			if (position < list.Length) {
				result = true;
				StringBuilder sb = new StringBuilder();
				do {
					if (list[position] == quote) {
						if (inQuote && position + 1 < list.Length && list[position + 1] == quote) {
							sb.Append(quote);
							sb.Append(quote);
							position += 2;
						}
						else {
							inQuote = !inQuote;
							sb.Append(quote);
							++position;
						}
					}
					else if (list[position] == separator && !inQuote) {
						++position;
						break;
					}
					else {
						sb.Append(list[position]);
						++position;
					}
				} while (position < list.Length);
				word = sb.ToString();
			}
			else {
				word = "";
				result = false;
			}
			return result;
		}


		internal static string Cut(string s, int length)
		{
			if (s.Length <= length) return s;
			else return s.Substring(0, length);
		}


		internal static string CalcRandomString(int minLength, int maxLength)
		{
			Random rand = new Random();
			int length = rand.Next(minLength, maxLength);
			StringBuilder result = new StringBuilder(length);
			for (int i = 0; i < length; ++i)
				result.Append((char) rand.Next((int) 'a', (int) 'z'));
			return result.ToString();
		}


		/// <summary>
		/// Prüft, ob s auf das Muster passt.
		/// </summary>
		/// <param name="pattern">String-Muster mit % und _ wie in SQL üblich</param>
		/// <param name="patternPos">Position im Muster, ab dem geprüft wird</param>
		/// <param name="str">Zu prüfender String</param>
		/// <param name="strPos">Position im String ab der geprüft wird</param>
		/// <param name="multiJoker">Ersatzzeichen für eine beliebige Anzahl Zeichen.</param>
		/// <param name="singleJoker">Ersatzzeichen für ein einzelnes Zeichen.</param>
		/// <param name="escape">Das zu verwendendende Escape-Zeichen</param>
		/// <param name="ignoreCase">Falls true, kein Unterschied zwischen Groß- und Kleinschreibung.</param>
		/// <returns></returns>
		internal static bool Matches(string pattern, int patternPos, string str, int strPos,
		                             char multiJoker, char singleJoker, char escape, bool ignoreCase)
		{
			bool result = true;
			while (result && patternPos < pattern.Length && strPos < str.Length) {
				// sowohl pp als auch sp zeigen auf ein gültiges Zeichen
				if (pattern[patternPos] == singleJoker) {
					++patternPos;
					++strPos;
				}
				else if (pattern[patternPos] == multiJoker) {
					// Wenn das % das letzte Zeichen im Muster ist oder einfach weggelassen werden kann, passt es
					if (patternPos == pattern.Length - 1
					    || Matches(pattern, patternPos + 1, str, strPos, multiJoker, singleJoker, escape, ignoreCase)) {
						result = true;
						patternPos = pattern.Length;
						strPos = str.Length;
					}
					else ++strPos;
				}
				else {
					if (pattern[patternPos] == escape && patternPos < pattern.Length - 1
					    && (pattern[patternPos + 1] == multiJoker || pattern[patternPos + 1] == singleJoker))
						++patternPos;
					if (ignoreCase) {
						if (ToUpper(str[strPos]) != ToUpper(pattern[patternPos])) result = false;
					}
					else {
						if (str[strPos] != pattern[patternPos]) result = false;
					}
					++patternPos;
					++strPos;
				}
			}
			if (result) {
				// Falls das Muster noch nicht fertig ist, können wir abschließende % übergehen
				while (patternPos < pattern.Length && pattern[patternPos] == multiJoker) ++patternPos;
				// Falls nicht beide strings am Ende sind, passt es nicht
				if (patternPos < pattern.Length || strPos < str.Length) result = false;
			}
			return result;
		}


		internal static string QuoteString(string s, char leftQuote, char rightQuote)
		{
			return leftQuote + s.Replace(new string(rightQuote, 1), new string(rightQuote, 2)) + rightQuote;
		}


		internal static string UnquoteString(string s, char leftQuote, char rightQuote)
		{
			Debug.Assert(s[0] == leftQuote && s[s.Length - 1] == rightQuote);
			return s.Replace(new string(leftQuote, 2), new string(rightQuote, 1));
		}


		private static byte[] upperMap;
		private static byte[] lowerMap;
		private static string umlautReplacementMap;
	}

	// StringExtensions


	internal class DateExtensions
	{
		static DateExtensions()
		{
			timeFormats = new string[4];
			timeFormats[0] = "H:mm";
			timeFormats[1] = "H:mm:ss.FFF";
			timeFormats[2] = "h:mm tt";
			timeFormats[3] = "h:mm:ss.FFF tt";
			dateFormats = new string[6];
			dateFormats[0] = "yyyy\\-M\\-d";
			dateFormats[1] = "yy\\-M\\-d";
			dateFormats[2] = "M\\/d\\/yyyy";
			dateFormats[3] = "M\\/d\\/yy";
			dateFormats[4] = "d\\.M\\.yyyy";
			dateFormats[5] = "d\\.M\\.yy";
			timestampFormats = new string[12];
			timestampFormats[0] = dateFormats[0] + " " + timeFormats[0];
			timestampFormats[1] = dateFormats[0] + " " + timeFormats[1];
			timestampFormats[2] = dateFormats[1] + " " + timeFormats[0];
			timestampFormats[3] = dateFormats[1] + " " + timeFormats[1];
			timestampFormats[4] = dateFormats[2] + " " + timeFormats[2];
			timestampFormats[5] = dateFormats[2] + " " + timeFormats[3];
			timestampFormats[6] = dateFormats[3] + " " + timeFormats[0];
			timestampFormats[7] = dateFormats[3] + " " + timeFormats[1];
			timestampFormats[8] = dateFormats[4] + " " + timeFormats[0];
			timestampFormats[9] = dateFormats[4] + " " + timeFormats[1];
			timestampFormats[10] = dateFormats[5] + " " + timeFormats[0];
			timestampFormats[11] = dateFormats[5] + " " + timeFormats[1];
		}


		// date ist das bitweise gepackte Format
		internal static DateTime TurboDBToDateTime(int packedDate, int milliSeconds)
		{
			DateTime result;
			if (packedDate == 0)
				result = DateTime.Today;
			else {
				int year = packedDate >> 9;
				int month = (packedDate >> 5)%16;
				int day = packedDate%32;
				result = new DateTime(year, month, day);
			}
			result.AddMilliseconds(milliSeconds);
			return result;
		}

		// TurboDBToDateTime


		/// <summary>
		/// Funktion ist fehlerhaft, wird aber derzeit nicht benutzt.
		/// 3.1.2005 -> 53!!!
		/// </summary>
		/// <param name="dt"></param>
		/// <returns></returns>
		internal static int WeekOfYear(DateTime dt)
		{
			int result;
			// Bestimme den Montag der ersten Woche im Jahr
			DateTime firstWeeksMonday = dt.AddMonths(1 - dt.Month).AddDays(1 - dt.Day);
			if (firstWeeksMonday.DayOfWeek == DayOfWeek.Sunday) firstWeeksMonday = firstWeeksMonday.AddDays(1);
			else if (firstWeeksMonday.DayOfWeek >= DayOfWeek.Thursday)
				firstWeeksMonday = firstWeeksMonday.AddDays(firstWeeksMonday.DayOfWeek - DayOfWeek.Thursday + 4);
			else firstWeeksMonday = firstWeeksMonday.AddDays(firstWeeksMonday.DayOfWeek - DayOfWeek.Monday);
			// Wenn das Datum davor liegt, greifen wir auf die Berechnung für das vorhergehende Jahr zurück
			if (dt < firstWeeksMonday) result = WeekOfYear(dt.AddDays(-7)) + 1;
			else result = dt.Subtract(firstWeeksMonday).Days/7 + 1;
			return result;
		}

		// WeekOfYear


		internal static string GetDateString(DateTime dt)
		{
			return dt.ToString("yyyy-MM-dd");
		}


		internal static string GetDateTimeString(DateTime dt, byte precision)
		{
			string format = "yyyy-MM-dd ";
			if (precision <= 2) format += "HH:mm";
			else if (precision <= 3) format += "HH:mm:ss";
			else format += "HH:mm:ss.fff";
			return dt.ToString(format);
		}


		internal static string GetTimeString(DateTime dt, byte precision)
		{
			string format;
			if (precision <= 2) format = "HH:mm";
			else if (precision <= 3) format = "HH:mm:ss";
			else format = "HH:mm:ss.fff";
			return dt.ToString(format);
		}


		/// <summary>
		/// Konvertiert s in ein Datum/Uhrzeit, wenn es einem der Standard TurboDB Datum/Uhrzeit-Formate
		/// entspricht.
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		internal static DateTime ParseTime(string s)
		{
			return DateTime.ParseExact(s, timeFormats, CultureInfo.InvariantCulture, DateTimeStyles.NoCurrentDateDefault);
		}


		internal static DateTime ParseDate(string s)
		{
			return DateTime.ParseExact(s, dateFormats, CultureInfo.InvariantCulture, DateTimeStyles.NoCurrentDateDefault);
		}


		internal static DateTime ParseTimestamp(string s)
		{
			return DateTime.ParseExact(s, timestampFormats, CultureInfo.InvariantCulture, DateTimeStyles.NoCurrentDateDefault);
		}


		/// <summary>
		/// Setzt voraus, dass s ein gültiges DateTime ist
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		internal static byte ParseDateTimePrecision(string s)
		{
			byte result = 0;
			int p = s.IndexOf(':');
			if (p >= 0) {
				p = s.IndexOf(':', p + 1);
				if (p >= 0) {
					p = s.IndexOf('.', p + 1);
					if (p >= 0) result = 4;
					else result = 3;
				}
				else result = 2;
			}
			else result = 1;
			return result;
		}


		private static string[] timeFormats;
		private static string[] dateFormats;
		private static string[] timestampFormats;
	}

	// DateExtensions


	internal class LogicalValue
	{
		internal static LogicalValue False = new LogicalValue();

		internal static LogicalValue Unknown = new LogicalValue();

		internal static LogicalValue True = new LogicalValue();


		internal static LogicalValue InvertLogicalValue(LogicalValue logicalValue)
		{
			if (logicalValue == LogicalValue.True) return LogicalValue.False;
			else if (logicalValue == LogicalValue.False) return LogicalValue.True;
			else return LogicalValue.Unknown;
		}


		public static LogicalValue operator &(LogicalValue v1, LogicalValue v2)
		{
			if (v1 == LogicalValue.False || v2 == LogicalValue.False) return LogicalValue.False;
			else if (v1 == LogicalValue.True && v2 == LogicalValue.True) return LogicalValue.True;
			else return LogicalValue.Unknown;
		}


		public static LogicalValue operator |(LogicalValue v1, LogicalValue v2)
		{
			if (v1 == LogicalValue.False && v2 == LogicalValue.False) return LogicalValue.False;
			else if (v1 == LogicalValue.True || v2 == LogicalValue.True) return LogicalValue.True;
			else return LogicalValue.Unknown;
		}


		/// <override></override>
		public override string ToString()
		{
			if (this == LogicalValue.False) return "LogicalValue.False";
			else if (this == LogicalValue.True) return "LogicalValue.True";
			else if (this == LogicalValue.Unknown) return "LogicalValue.NotSupported";
			else {
				Debug.Fail("Unexpected logical value");
				return "Unsupported logical value";
			}
		}
	}


	/// <ToBeCompleted></ToBeCompleted>
	public static class Diagnostics
	{
		/// <ToBeCompleted></ToBeCompleted>
		public static void RuntimeCheck(bool condition, string message)
		{
#if DEBUG
			if (!condition) throw new InvalidOperationException(message);
#endif
		}
	}
}