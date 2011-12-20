using System;
using System.Runtime.Serialization;

// These exist for these reasons:
// Parameters of like types can be differentiated.
// Limit and/or transform values.

namespace Vixen.Commands.KnownDataTypes {
	#region Level
	[DataContract]
	public struct Level {
		static public readonly int MinValue = 0;
		static public readonly int MaxValue = 100;

		[DataMember]
		private double _value;

		private Level(double value) {
			if(value < MinValue) {
				value = MinValue;
			} else if(value > MaxValue) {
				value = MaxValue;
			}
			_value = value;
		}

		// Level to double conversion.
		// Ex:
		// Level level;
		// double x = level;
		public static implicit operator double(Level value) {
			return value._value;
		}

		// Double to level conversion.
		// Ex:
		// double x;
		// Level level = x;
		public static implicit operator Level(double value) {
			return new Level(value);
		}
	}
	#endregion

	#region Position
	[DataContract]
	public struct Position {
		static public readonly int MinValue = 0;
		static public readonly int MaxValue = 100;

		[DataMember]
		private double _value;

		private Position(double value) {
			if(value < MinValue) {
				value = MinValue;
			} else if(value > MaxValue) {
				value = MaxValue;
			}
			_value = value;
		}

		public static implicit operator double(Position value) {
			return value._value;
		}

		public static implicit operator Position(double value) {
			return new Position(value);
		}

		public override string ToString() {
			return _value.ToString();
		}
	}
	#endregion

}
