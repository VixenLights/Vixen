using System;
using Vixen.Commands;
using Vixen.Data.Value;

namespace Vixen.Data.Combinator._8Bit {
	public class _8BitHighestWinsCombinator : Combinator<_8BitHighestWinsCombinator, byte> {
		public override void Handle(_8BitCommand obj) {
			if(CombinatorValue == null) {
				CombinatorValue = obj;
			} else {
				byte value1 = CombinatorValue.CommandValue;
				byte value2 = obj.CommandValue;
				CombinatorValue = new _8BitCommand(Math.Max(value1, value2));
			}
		}

		public override void Handle(ColorCommand obj) {
			if(CombinatorValue == null) {
				CombinatorValue = new _8BitCommand(ColorValue.GetGrayscaleLevel(obj.CommandValue));
			} else {
				byte value1 = CombinatorValue.CommandValue;
				byte value2 = ColorValue.GetGrayscaleLevel(obj.CommandValue);
				CombinatorValue = new _8BitCommand(Math.Max(value1, value2));
			}
		}

		public override void Handle(_32BitCommand obj) {
			if(CombinatorValue == null) {
				CombinatorValue = new _8BitCommand(obj.CommandValue);
			} else {
				byte value1 = CombinatorValue.CommandValue;
				byte value2 = (byte)obj.CommandValue;
				CombinatorValue = new _8BitCommand(Math.Max(value1, value2));
			}
		}

		public override void Handle(_64BitCommand obj) {
			if(CombinatorValue == null) {
				CombinatorValue = new _8BitCommand(obj.CommandValue);
			} else {
				byte value1 = CombinatorValue.CommandValue;
				byte value2 = (byte)obj.CommandValue;
				CombinatorValue = new _8BitCommand(Math.Max(value1, value2));
			}
		}

		public override void Handle(_16BitCommand obj) {
			if(CombinatorValue == null) {
				CombinatorValue = new _8BitCommand(obj.CommandValue);
			} else {
				byte value1 = CombinatorValue.CommandValue;
				byte value2 = (byte)obj.CommandValue;
				CombinatorValue = new _8BitCommand(Math.Max(value1, value2));
			}
		}
	}
}
