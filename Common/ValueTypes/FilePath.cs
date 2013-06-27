using System;
using System.Runtime.Serialization;

namespace Common.ValueTypes
{
	[DataContract]
	public struct FilePath : IEquatable<FilePath>
	{
		public FilePath(string value)
		{
			Value = value;
		}

		[DataMember] public readonly string Value;

		#region Implicit Operators

		public static implicit operator string(FilePath filePath)
		{
			return filePath.Value;
		}

		public static implicit operator FilePath(string value)
		{
			return new FilePath(value);
		}

		#endregion

		#region Equality

		public bool Equals(FilePath other)
		{
			return Equals(other.Value, Value);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (obj.GetType() != typeof (FilePath)) return false;
			return Equals((FilePath) obj);
		}

		public override int GetHashCode()
		{
			return (Value != null ? Value.GetHashCode() : 0);
		}

		public static bool operator ==(FilePath left, FilePath right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(FilePath left, FilePath right)
		{
			return !left.Equals(right);
		}

		#endregion
	}
}