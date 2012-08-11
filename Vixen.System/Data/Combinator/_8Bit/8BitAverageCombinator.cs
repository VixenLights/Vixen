using Vixen.Commands;
using Vixen.Sys;

namespace Vixen.Data.Combinator._8Bit {
	public class _8BitAverageCombinator : Combinator<_8BitAverageCombinator, byte> {
		public override void Handle(IEvaluator<byte> obj) {
			if(CombinatorValue == null) {
				CombinatorValue = obj.EvaluatorValue;
			} else {
				byte value1 = CombinatorValue.CommandValue;
				byte value2 = obj.EvaluatorValue.CommandValue;
				CombinatorValue = new _8BitCommand((value1 + value2) >> 1);
			}
		}
	}
}
