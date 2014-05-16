using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Vixen.Module;

namespace VixenModules.Sequence.Timed
{
	[DataContract]
	public class TimedSequenceData : BaseSequence.SequenceData
	{
		[DataMember]
		public List<MarkCollection> MarkCollections { get; set; }

		[DataMember]
		public TimeSpan TimePerPixel { get; set; }
	
		public TimedSequenceData()
		{
			MarkCollections = new List<MarkCollection>();
			TimePerPixel = TimeSpan.MinValue;
		}

		public override IModuleDataModel Clone()
		{
			TimedSequenceData result = new TimedSequenceData();
			// Cloning each MarkCollection so that the cloned data objects don't share references
			// and step on each other.
			result.MarkCollections = new List<MarkCollection>(MarkCollections.Select(x => new MarkCollection(x)));
			return result;
		}
	}
}