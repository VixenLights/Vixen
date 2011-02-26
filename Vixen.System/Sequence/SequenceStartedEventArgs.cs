using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Common;

namespace Vixen.Sequence {
    public class SequenceStartedEventArgs : EventArgs {
        public SequenceStartedEventArgs(ITimingSource timingSource) {
            this.TimingSource = timingSource;
        }

        public ITimingSource TimingSource { get; private set; }
    }
}
