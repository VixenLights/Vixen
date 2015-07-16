using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Common.ValueTypes
{
	[DataContract]
	[TypeConverter(typeof(PercentageTypeConverter))]
	public struct Percentage : IEquatable<Percentage>
	{
		public Percentage(float value)
		{
			if (value < 0 || value > 1) throw new ArgumentException("Percentage value must be between 0 and 1.");

			Value = value;
		}

		[DataMember] public readonly float Value;

		#region Implicit Operators

		public static implicit operator float(Percentage percentage)
		{
			return percentage.Value;
		}

		public static implicit operator Percentage(float value)
		{
			return new Percentage(value);
		}

		public static implicit operator double(Percentage percentage)
		{
			return percentage.Value;
		}

		public static implicit operator Percentage(double value)
		{
			return new Percentage((float) value);
		}

		#endregion

		#region Equality

		public bool Equals(Percentage other)
		{
			return other.Value.Equals(Value);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (obj.GetType() != typeof (Percentage)) return false;
			return Equals((Percentage) obj);
		}

		public override int GetHashCode()
		{
			return Value.GetHashCode();
		}

		public static bool operator ==(Percentage left, Percentage right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(Percentage left, Percentage right)
		{
			return !left.Equals(right);
		}

		#endregion
	}
}