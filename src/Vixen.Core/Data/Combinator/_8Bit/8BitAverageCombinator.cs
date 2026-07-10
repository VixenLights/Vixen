using Vixen.Commands;

namespace Vixen.Data.Combinator._8Bit
{
	public class _8BitAverageCombinator : Combinator<_8BitAverageCombinator>
	{
		public override void Handle(_8BitCommand obj)
		{
			if (CombinatorValue == null) {
				CombinatorValue = obj;
			}
			else {
				byte value1 = (CombinatorValue as _8BitCommand).CommandValue;
				byte value2 = obj.CommandValue;
				CombinatorValue = new _8BitCommand((value1 + value2) >> 1);
			}
		}
	}
}