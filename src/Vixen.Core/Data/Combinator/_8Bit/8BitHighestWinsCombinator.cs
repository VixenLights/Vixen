﻿using Vixen.Commands;
using Vixen.Data.Value;

namespace Vixen.Data.Combinator._8Bit
{
	public class _8BitHighestWinsCombinator : Combinator<_8BitHighestWinsCombinator>
	{
		public override void Handle(_8BitCommand obj)
		{
			if (CombinatorValue == null) {
				CombinatorValue = obj;
			}
			else {
				CombinatorValue = obj.Max((_8BitCommand) CombinatorValue);
			}
		}

		public override void Handle(ColorCommand obj)
		{
			if (CombinatorValue == null) {
				CombinatorValue = new _8BitCommand(RGBValue.GetGrayscaleLevel(obj.CommandValue));
			}
			else {
				byte value1 = (CombinatorValue as _8BitCommand).CommandValue;
				byte value2 = RGBValue.GetGrayscaleLevel(obj.CommandValue);
				CombinatorValue = new _8BitCommand(Math.Max(value1, value2));
			}
		}

		public override void Handle(_32BitCommand obj)
		{
			if (CombinatorValue == null) {
				CombinatorValue = new _8BitCommand(obj.CommandValue);
			}
			else {
				byte value1 = (CombinatorValue as _8BitCommand).CommandValue;
				byte value2 = (byte) obj.CommandValue;
				CombinatorValue = new _8BitCommand(Math.Max(value1, value2));
			}
		}

		public override void Handle(_64BitCommand obj)
		{
			if (CombinatorValue == null) {
				CombinatorValue = new _8BitCommand(obj.CommandValue);
			}
			else {
				byte value1 = (CombinatorValue as _8BitCommand).CommandValue;
				byte value2 = (byte) obj.CommandValue;
				CombinatorValue = new _8BitCommand(Math.Max(value1, value2));
			}
		}

		public override void Handle(_16BitCommand obj)
		{
			if (CombinatorValue == null) {
				CombinatorValue = new _8BitCommand(obj.CommandValue);
			}
			else {
				byte value1 = (CombinatorValue as _8BitCommand).CommandValue;
				byte value2 = (byte) obj.CommandValue;
				CombinatorValue = new _8BitCommand(Math.Max(value1, value2));
			}
		}
	}
}