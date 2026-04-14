using Vixen.Commands;

namespace Vixen.Data.Combinator
{
	// Discards any excess command values: effectively, it always keeps the last Command it is given.
	public class DiscardExcessCombinator : Combinator<DiscardExcessCombinator>
	{
		public override void Handle(_16BitCommand obj)
		{
			CombinatorValue = obj;
		}

		public override void Handle(_32BitCommand obj)
		{
			CombinatorValue = obj;
		}

		public override void Handle(_64BitCommand obj)
		{
			CombinatorValue = obj;
		}

		public override void Handle(_8BitCommand obj)
		{
			CombinatorValue = obj;
		}

		public override void Handle(ColorCommand obj)
		{
			CombinatorValue = obj;
		}

		public override void Handle(StringCommand obj)
		{
			CombinatorValue = obj;
		}
	}
}
