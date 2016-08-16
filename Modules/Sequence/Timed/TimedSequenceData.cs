using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Vixen.Module;
using Vixen.Sys.LayerMixing;

namespace VixenModules.Sequence.Timed
{
	[DataContract]
	public class TimedSequenceData : BaseSequence.SequenceData
	{
		[DataMember]
		public List<MarkCollection> MarkCollections { get; set; }

		[DataMember]
		public TimeSpan TimePerPixel { get; set; }

		[DataMember]
		public TimeSpan VisibleTimeStart { get; set; }

		[DataMember]
		public int DefaultRowHeight { get; set; }

		[DataMember]
		public int DefaultSplitterDistance { get; set; }

		[DataMember]
		public RowSettings RowSettings { get; set; }
		

		[DataMember]
		public TimeSpan? DefaultPlaybackEndTime { get; set; }

		[DataMember]
		public TimeSpan? DefaultPlaybackStartTime { get; set; }

		public TimedSequenceData()
		{
			MarkCollections = new List<MarkCollection>();
			TimePerPixel = TimeSpan.MinValue;
			DefaultRowHeight = 0;
			RowSettings = new RowSettings();
			VisibleTimeStart = TimeSpan.MinValue;
			DefaultSplitterDistance = 0;
			DefaultPlaybackStartTime = TimeSpan.Zero;
			DefaultPlaybackEndTime = TimeSpan.Zero;
		}

		[OnDeserialized]
		void OnDeserialized(StreamingContext c)
		{
			if (RowSettings == null)
			{
				RowSettings = new RowSettings();
			}
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