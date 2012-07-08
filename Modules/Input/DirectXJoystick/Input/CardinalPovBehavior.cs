using System;
using System.Linq;
using SlimDX.DirectInput;

namespace VixenModules.Input.DirectXJoystick.Input {
	class CardinalPovBehavior : IPovBehavior {
		private Range[] _directionRanges;

		public enum Direction { N = 0, S = 18000, E = 9000, W = 27000 };

		public CardinalPovBehavior() {
			// No dead zones.
			_directionRanges = new[] {
				new Range(0, 45000, (int)Direction.N),
				new Range(45000, 90000, (int)Direction.E),
				new Range(135000, 90000, (int)Direction.S),
				new Range(225000, 90000, (int)Direction.W),
				new Range(315000, 45000, (int)Direction.N)
			};
		}

		public double GetValue(JoystickState joystickState, int povIndex) {
			// Position is quantized degrees from North quantized to the four cardinal directions.
			int position = joystickState.GetPointOfViewControllers()[povIndex];
			foreach(Range range in _directionRanges) {
				if(range.Contains(position)) {
					return range.Value / 100d;
				}
			}

			return -1;
		}


		private class Range {
			public Range(int low, int sweep, int value) {
				Low = low;
				Sweep = sweep;
				High = low + sweep;
				Value = value;
			}

			public int Low { get; private set; }
			public int High { get; private set; }
			public int Sweep { get; private set; }
			public int Value { get; private set; }

			public bool Contains(int value) {
				return value >= Low && value <= High;
			}
		}
	}
}
