using System;
using Vixen.Commands;
using Vixen.Data.Value;

namespace Vixen.Data.Combinator._16Bit
{
	public class _16BitHighestWinsCombinator : Combinator<_16BitHighestWinsCombinator>
	{
		public override void Handle(_8BitCommand obj)
		{
			if (CombinatorValue == null) {
				CombinatorValue = new _16BitCommand(obj.CommandValue);
			}
			else {
				ushort value1 = (CombinatorValue as _16BitCommand).CommandValue;
				ushort value2 = obj.CommandValue;
				CombinatorValue = new _16BitCommand(Math.Max(value1, value2));
			}
		}

		public override void Handle(ColorCommand obj)
		{
			if (CombinatorValue == null) {
				CombinatorValue = new _16BitCommand(ColorValue.GetGrayscaleLevel(obj.CommandValue));
			}
			else {
				ushort value1 = (CombinatorValue as _16BitCommand).CommandValue;
				ushort value2 = ColorValue.GetGrayscaleLevel(obj.CommandValue);
				CombinatorValue = new _16BitCommand(Math.Max(value1, value2));
			}
		}

		public override void Handle(_32BitCommand obj)
		{
			if (CombinatorValue == null) {
				CombinatorValue = new _16BitCommand(obj.CommandValue);
			}
			else {
				ushort value1 = (CombinatorValue as _16BitCommand).CommandValue;
				ushort value2 = (ushort) obj.CommandValue;
				CombinatorValue = new _16BitCommand(Math.Max(value1, value2));
			}
		}

		public override void Handle(_64BitCommand obj)
		{
			if (CombinatorValue == null) {
				CombinatorValue = new _16BitCommand(obj.CommandValue);
			}
			else {
				ushort value1 = (CombinatorValue as _16BitCommand).CommandValue;
				ushort value2 = (ushort) obj.CommandValue;
				CombinatorValue = new _16BitCommand(Math.Max(value1, value2));
			}
		}

		public override void Handle(_16BitCommand obj)
		{
			if (CombinatorValue == null) {
				CombinatorValue = obj;
			}
			else {
				ushort value1 = (CombinatorValue as _16BitCommand).CommandValue;
				ushort value2 = obj.CommandValue;
				CombinatorValue = new _16BitCommand(Math.Max(value1, value2));
			}
		}
	}
}