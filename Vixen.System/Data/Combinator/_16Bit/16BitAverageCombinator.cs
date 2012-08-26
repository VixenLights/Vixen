using Vixen.Commands;

namespace Vixen.Data.Combinator._16Bit {
	public class _16BitAverageCombinator : Combinator<_16BitAverageCombinator, ushort> {
		public override void Handle(_16BitCommand obj) {
			if(CombinatorValue == null) {
				CombinatorValue = obj;
			} else {
				ushort value1 = CombinatorValue.CommandValue;
				ushort value2 = obj.CommandValue;
				CombinatorValue = new _16BitCommand((value1 + value2) >> 1);
			}
		}
	}
}
