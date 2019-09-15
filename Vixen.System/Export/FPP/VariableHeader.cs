using System;
using System.Text;

namespace Vixen.Export.FPP
{
	public class VariableHeader
	{
		public VariableHeader(HeaderType type)
		{
			switch (type)
			{
				case HeaderType.MediaFile:
					Code1 = 'm';
					Code2 = 'f';
					break;
				case HeaderType.SequenceProducer:
					Code1 = 's';
					Code2 = 'p';
					Value = @"Vixen 3";
					break;
			}	
		}

		public VariableHeader(string mediaName):this(HeaderType.MediaFile)
		{
			Value = mediaName;
		}

		public char Code1 { get; set; }

		public char Code2 { get; set; }

		public string Value { get; set; }

		private byte LengthLowByte => AsLowByte(Value.Length + 5);

		private byte LengthHighByte => AsHighByte(Value.Length + 5);

		public int HeaderLength => Value.Length + 5;

		public byte[] GetHeaderBytes()
		{
			var bytes = new byte[Value.Length + 5];
			bytes[0] = LengthLowByte;
			bytes[1] = LengthHighByte;
			bytes[2] = (byte)Code1;
			bytes[3] = (byte)Code2;
			
			Array.Copy(Encoding.ASCII.GetBytes(Value), 0, bytes, 4, Value.Length);

			bytes[bytes.Length -1] = Byte.MinValue;

			return bytes;
		}

		private byte AsLowByte(int i)
		{
			return (byte) (i & 0xFF);
		}

		private byte AsHighByte(int i)
		{
			return (byte) ((i >> 8) & 0xFF);
		}
	}

	public enum HeaderType
	{
		MediaFile,
		SequenceProducer
	}
}
