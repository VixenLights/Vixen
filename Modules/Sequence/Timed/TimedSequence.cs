using System.Collections.Generic;

namespace VixenModules.Sequence.Timed {
	public class TimedSequence : BaseSequence.Sequence
	{
		public static string Extension = ".tim";

		public List<MarkCollection> MarkCollections {
			get { return ((TimedSequenceData)SequenceData).MarkCollections; }
			set { ((TimedSequenceData)SequenceData).MarkCollections = value; }
		}

		public override string FileExtension
		{
			get { return Extension; }
		}
	}
}
