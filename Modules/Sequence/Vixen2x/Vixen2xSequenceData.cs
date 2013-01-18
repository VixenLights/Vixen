using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Vixen.Module;

namespace VixenModules.Sequence.Vixen2x
{
	[DataContract]
	public class Vixen2xSequenceData : BaseSequence.SequenceData
	{
		[DataMember]
		public List<MarkCollection> MarkCollections { get; set; }

		public Vixen2xSequenceData()
		{
			MarkCollections = new List<MarkCollection>();
		}

		public override IModuleDataModel Clone()
		{
			Vixen2xSequenceData result = new Vixen2xSequenceData();
			// Cloning each MarkCollection so that the cloned data objects don't share references
			// and step on each other.
			result.MarkCollections = new List<MarkCollection>(MarkCollections.Select(x => new MarkCollection(x)));
			return result;
		}
	}
}
