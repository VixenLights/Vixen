using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using CommandStandard.Types;
using System.Runtime.Serialization;

namespace Vixen.Sys {
	static class ParameterValue {
		private enum DataType : byte { Level, Color, Time, Position, String, Value };

		static public void WriteToStream(Stream stream, Level value) {
			stream.WriteByte((byte)DataType.Level);
			byte[] bytes = BitConverter.GetBytes(value);
			stream.Write(bytes, 0, bytes.Length);
		}

		static public void WriteToStream(Stream stream, Color value) {
			stream.WriteByte((byte)DataType.Color);
			byte[] bytes = BitConverter.GetBytes(value);
			stream.Write(bytes, 0, bytes.Length);
		}

		static public void WriteToStream(Stream stream, Time value) {
			stream.WriteByte((byte)DataType.Time);
			byte[] bytes = BitConverter.GetBytes(value);
			stream.Write(bytes, 0, bytes.Length);
		}

		static public void WriteToStream(Stream stream, Position value) {
			stream.WriteByte((byte)DataType.Position);
			byte[] bytes = BitConverter.GetBytes(value);
			stream.Write(bytes, 0, bytes.Length);
		}

		static public void WriteToStream(Stream stream, double value) {
			stream.WriteByte((byte)DataType.Value);
			byte[] bytes = BitConverter.GetBytes(value);
			stream.Write(bytes, 0, bytes.Length);
		}

		static public void WriteToStream(Stream stream, string value) {
			stream.WriteByte((byte)DataType.String);
			ushort length = (ushort)value.Length;
			// Length of the string
			byte[] bytes = BitConverter.GetBytes(length);
			stream.Write(bytes, 0, bytes.Length);
			// String itself
			bytes = Encoding.ASCII.GetBytes(value);
			stream.Write(bytes, 0, bytes.Length);
		}

		static public void WriteToStream(Stream stream, object value) {
			if(value is string) {
				WriteToStream(stream, value as string);
			} else if(value is Level) {
				WriteToStream(stream, (Level)value);
			} else if(value is Color) {
				WriteToStream(stream, (Color)value);
			} else if(value is Time) {
				WriteToStream(stream, (Time)value);
			} else if(value is Position) {
				WriteToStream(stream, (Position)value);
			} else if(value is ValueType) {
				WriteToStream(stream, Convert.ToDouble(value));
			} else {
				throw new ArgumentException("Parameter must be a string or numeric value type");
			}
		}

		static public object ReadFromStream(Stream stream) {
			switch((DataType)stream.ReadByte()) {
				case DataType.Level:
					return _ReadLevelFromStream(stream);
				case DataType.Color:
					return _ReadColorFromStream(stream);
				case DataType.Time:
					return _ReadTimeFromStream(stream);
				case DataType.Position:
					return _ReadPositionFromStream(stream);
				case DataType.String:
					return _ReadStringFromStream(stream);
				default:
					return _ReadValueTypeFromStream(stream);
			}
		}

		static private Level _ReadLevelFromStream(Stream stream) {
			byte[] bytes = new byte[sizeof(double)];
			stream.Read(bytes, 0, bytes.Length);
			return BitConverter.ToDouble(bytes, 0);
		}

		static private Color _ReadColorFromStream(Stream stream) {
			byte[] bytes = new byte[sizeof(double)];
			stream.Read(bytes, 0, bytes.Length);
			return BitConverter.ToInt32(bytes, 0);
		}

		static private Time _ReadTimeFromStream(Stream stream) {
			byte[] bytes = new byte[sizeof(double)];
			stream.Read(bytes, 0, bytes.Length);
			return BitConverter.ToInt32(bytes, 0);
		}

		static private Position _ReadPositionFromStream(Stream stream) {
			byte[] bytes = new byte[sizeof(double)];
			stream.Read(bytes, 0, bytes.Length);
			return BitConverter.ToDouble(bytes, 0);
		}

		static private double _ReadValueTypeFromStream(Stream stream) {
			byte[] bytes = new byte[sizeof(double)];
			stream.Read(bytes, 0, bytes.Length);
			return BitConverter.ToDouble(bytes, 0);
		}

		static private string _ReadStringFromStream(Stream stream) {
			int length = sizeof(ushort);
			byte[] bytes = new byte[length];
			// Length of the string (ushort)
			stream.Read(bytes, 0, length);
			ushort stringLength = BitConverter.ToUInt16(bytes, 0);
			bytes = new byte[stringLength];
			// String itself
			stream.Read(bytes, 0, stringLength);
			return Encoding.ASCII.GetString(bytes);
		}
	}
}
