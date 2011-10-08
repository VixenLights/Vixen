using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;
using Vixen.Module.Timing;

namespace Vixen.Sys {
    public class SequenceStartedEventArgs : EventArgs {
        public SequenceStartedEventArgs(ISequence sequence, ITiming timingSource) {
			Sequence = sequence;
            TimingSource = timingSource;
        }

		public ISequence Sequence { get; private set; }
		public ITiming TimingSource { get; private set; }
    }
}
