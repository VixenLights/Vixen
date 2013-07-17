using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Commands;
using Vixen.Data.Value;

namespace Vixen.Data.Combinator.Commands
{
	public class DynamicCombinator : Combinator<DynamicCombinator, DynamicValue>
	{
		public override void Handle(DynamicCommand obj)
		{
			CombinatorValue = new DynamicCommand(obj.CommandValue);
		}
	}
}
