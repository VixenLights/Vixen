using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Common;
using Vixen.Module.Timing;

namespace Vixen.Sys {
    public class SequenceStartedEventArgs : EventArgs {
        public SequenceStartedEventArgs(ITiming timingSource) {
            this.TimingSource = timingSource;
        }

		public ITiming TimingSource { get; private set; }
    }
}
