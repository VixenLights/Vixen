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

		public CacheStartedEventArgs(ISequence sequence, ITiming timingSource, TimeSpan startTime, TimeSpan endTime)
		{
			Sequence = sequence;
			TimingSource = timingSource;
			StartTime = startTime;
			EndTime = endTime;
		}

		public ISequence Sequence { get; private set; }
		public ITiming TimingSource { get; private set; }
		public TimeSpan StartTime { get; private set; }
		public TimeSpan EndTime { get; private set; }
	}
}
