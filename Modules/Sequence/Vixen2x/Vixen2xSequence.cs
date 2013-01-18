using System.Collections.Generic;

namespace VixenModules.Sequence.Vixen2x {
	public class Vixen2xSequence : BaseSequence.Sequence {

		public List<MarkCollection> MarkCollections	{
			get { return ((Vixen2xSequenceData)SequenceData).MarkCollections; }
			set { ((Vixen2xSequenceData)SequenceData).MarkCollections = value; }
		}
	}
}