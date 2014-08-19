using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;

namespace Vixen.Cache.Event
{
	public class CacheEventArgs: EventArgs
	{
		public CacheEventArgs(ISequence sequence)
		{
			Sequence = sequence;
		}

		public ISequence Sequence { get; private set; }
	}
}
