using System.Collections.Generic;

namespace VixenModules.Sequence.Timed {
	public class TimedSequence : BaseSequence.Sequence {
		public List<MarkCollection> MarkCollections {
			get { return ((TimedSequenceData)SequenceData).MarkCollections; }
			set { ((TimedSequenceData)SequenceData).MarkCollections = value; }
		}
	}
}
