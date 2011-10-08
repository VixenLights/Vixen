using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Runtime.Serialization;

// These exist for these reasons:
// Parameters of like types can be differentiated.
// Limit and/or transform values.

namespace Vixen.Commands.KnownDataTypes {
	#region Level
	[DataContract]
	public struct Level {
		[DataMember]
		private double _value;

		private Level(double value) {
			if(value < 0) {
				value = 0;
			} else if(value > 100) {
				value = 100;
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

		public override string ToString() {
			return _value.ToString();
		}
	}
	#endregion

	//#region Time
	//public struct Time {
	//    private int _value;

	//    private Time(int value) {
	//        _value = value;
	//    }

	//    public static implicit operator int(Time value) {
	//        return value._value;
	//    }

	//    public static implicit operator Time(int value) {
	//        return new Time(value);
	//    }

	//    public override string ToString() {
	//        return _value.ToString();
	//    }
	//}
	//#endregion

	#region Position
	[DataContract]
	public struct Position {
		[DataMember]
		private double _value;

		private Position(double value) {
			if(value < 0 || value > 100) throw new InvalidOperationException("Position value must be between 0 and 100 percent.");
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
