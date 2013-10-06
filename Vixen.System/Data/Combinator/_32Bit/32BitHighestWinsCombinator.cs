using System;
using Vixen.Commands;
using Vixen.Data.Value;

namespace Vixen.Data.Combinator._32Bit
{
	public class _32BitHighestWinsCombinator : Combinator<_32BitHighestWinsCombinator>
	{
		public override void Handle(_8BitCommand obj)
		{
			if (CombinatorValue == null) {
				CombinatorValue = new _32BitCommand(obj.CommandValue);
			}
			else {
				uint value1 = (CombinatorValue as _32BitCommand).CommandValue;
				uint value2 = obj.CommandValue;
				CombinatorValue = new _32BitCommand(Math.Max(value1, value2));
			}
		}

		public override void Handle(ColorCommand obj)
		{
			if (CombinatorValue == null) {
				CombinatorValue = new _32BitCommand(RGBValue.GetGrayscaleLevel(obj.CommandValue));
			}
			else {
				uint value1 = (CombinatorValue as _32BitCommand).CommandValue;
				uint value2 = RGBValue.GetGrayscaleLevel(obj.CommandValue);
				CombinatorValue = new _32BitCommand(Math.Max(value1, value2));
			}
		}

		public override void Handle(_32BitCommand obj)
		{
			if (CombinatorValue == null) {
				CombinatorValue = obj;
			}
			else {
				uint value1 = (CombinatorValue as _32BitCommand).CommandValue;
				uint value2 = (byte) obj.CommandValue;
				CombinatorValue = new _32BitCommand(Math.Max(value1, value2));
			}
		}

		public override void Handle(_64BitCommand obj)
		{
			if (CombinatorValue == null) {
				CombinatorValue = new _32BitCommand(obj.CommandValue);
			}
			else {
				uint value1 = (CombinatorValue as _32BitCommand).CommandValue;
				uint value2 = (byte) obj.CommandValue;
				CombinatorValue = new _32BitCommand(Math.Max(value1, value2));
			}
		}

		public override void Handle(_16BitCommand obj)
		{
			if (CombinatorValue == null) {
				CombinatorValue = new _32BitCommand(obj.CommandValue);
			}
			else {
				uint value1 = (CombinatorValue as _32BitCommand).CommandValue;
				uint value2 = (byte) obj.CommandValue;
				CombinatorValue = new _32BitCommand(Math.Max(value1, value2));
			}
		}
	}
}