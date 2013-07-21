using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.Data.Combinator
{
	// Discards any excess command values: effectively, it always keeps the last Command it is given.
	public class DiscardExcessCombinator : Combinator<DiscardExcessCombinator>
	{
		public override void Handle(Commands._16BitCommand obj)
		{
			CombinatorValue = obj;
		}

		public override void Handle(Commands._32BitCommand obj)
		{
			CombinatorValue = obj;
		}

		public override void Handle(Commands._64BitCommand obj)
		{
			CombinatorValue = obj;
		}

		public override void Handle(Commands._8BitCommand obj)
		{
			CombinatorValue = obj;
		}

		public override void Handle(Commands.ColorCommand obj)
		{
			CombinatorValue = obj;
		}

		public override void Handle(Commands.StringCommand obj)
		{
			CombinatorValue = obj;
		}
	}
}
