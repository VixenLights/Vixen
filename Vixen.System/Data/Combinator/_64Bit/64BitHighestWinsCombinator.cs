using System;
using Vixen.Commands;
using Vixen.Data.Value;

namespace Vixen.Data.Combinator._64Bit
{
	public class _64BitHighestWinsCombinator : Combinator<_64BitHighestWinsCombinator>
	{
		public override void Handle(_8BitCommand obj)
		{
			if (CombinatorValue == null) {
				CombinatorValue = new _64BitCommand(obj.CommandValue);
			}
			else {
				ulong value1 = (CombinatorValue.CommandValue as _64BitCommand).CommandValue;
				ulong value2 = obj.CommandValue;
				CombinatorValue = new _64BitCommand(Math.Max(value1, value2));
			}
		}

		public override void Handle(ColorCommand obj)
		{
			if (CombinatorValue == null) {
				CombinatorValue = new _64BitCommand(ColorValue.GetGrayscaleLevel(obj.CommandValue));
			}
			else {
				ulong value1 = (CombinatorValue.CommandValue as _64BitCommand).CommandValue;
				ulong value2 = ColorValue.GetGrayscaleLevel(obj.CommandValue);
				CombinatorValue = new _64BitCommand(Math.Max(value1, value2));
			}
		}

		public override void Handle(_32BitCommand obj)
		{
			if (CombinatorValue == null) {
				CombinatorValue = new _64BitCommand(obj.CommandValue);
			}
			else {
				ulong value1 = (CombinatorValue.CommandValue as _64BitCommand).CommandValue;
				ulong value2 = (byte) obj.CommandValue;
				CombinatorValue = new _64BitCommand(Math.Max(value1, value2));
			}
		}

		public override void Handle(_64BitCommand obj)
		{
			if (CombinatorValue == null) {
				CombinatorValue = obj;
			}
			else {
				ulong value1 = (CombinatorValue.CommandValue as _64BitCommand).CommandValue;
				ulong value2 = (byte) obj.CommandValue;
				CombinatorValue = new _64BitCommand(Math.Max(value1, value2));
			}
		}

		public override void Handle(_16BitCommand obj)
		{
			if (CombinatorValue == null) {
				CombinatorValue = new _64BitCommand(obj.CommandValue);
			}
			else {
				ulong value1 = (CombinatorValue.CommandValue as _64BitCommand).CommandValue;
				ulong value2 = (byte) obj.CommandValue;
				CombinatorValue = new _64BitCommand(Math.Max(value1, value2));
			}
		}
	}
}