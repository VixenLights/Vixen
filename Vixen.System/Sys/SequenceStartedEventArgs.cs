using System;
using Vixen.Module.Timing;

namespace Vixen.Sys {
    public class SequenceStartedEventArgs : EventArgs {
        public SequenceStartedEventArgs(ISequence sequence, ITiming timingSource, TimeSpan startTime, TimeSpan endTime) {
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
