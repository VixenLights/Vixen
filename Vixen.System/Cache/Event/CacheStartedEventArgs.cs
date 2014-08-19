using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module.Timing;
using Vixen.Sys;

namespace Vixen.Cache.Event
{
	public class CacheStartedEventArgs : EventArgs
	{

		public CacheStartedEventArgs(ITiming timingSource, TimeSpan length, TimeSpan start)
		{
			Start = start;
			Length = length;
			TimingSource = timingSource;
		}

		public TimeSpan Start { get; private set; }
		public TimeSpan Length { get; private set; }
		public ITiming TimingSource { get; private set; }
	}
}
