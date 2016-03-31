using System;
using System.Collections.Generic;
using Vixen.Sys.Range;

namespace Vixen.Sys
{
	public class IntentNodeCollection : List<IIntentNode>
	{
		public IntentNodeCollection(IEnumerable<IIntentNode> intentNodes)
		{
			AddRange(intentNodes);
		}
	}

}