using System;
using System.Collections.ObjectModel;
using Vixen.Marks;
using Vixen.Sys.LayerMixing;

namespace VixenModules.Sequence.Timed
{
	public class TimedSequence : BaseSequence.Sequence
	{
		public static string Extension = ".tim";

		public override ObservableCollection<IMarkCollection> LabeledMarkCollections => ((TimedSequenceData)SequenceData).LabeledMarkCollections;

		public RowSettings RowSettings
		{
			get { return ((TimedSequenceData)SequenceData).RowSettings; }
			set { ((TimedSequenceData)SequenceData).RowSettings = value; }
		}

		public override string FileExtension
		{
			get { return Extension; }
		}

		public TimeSpan TimePerPixel
		{
			get { return ((TimedSequenceData) SequenceData).TimePerPixel; }
			set { ((TimedSequenceData)SequenceData).TimePerPixel = value; }
		}

		public TimeSpan VisibleTimeStart
		{
			get { return ((TimedSequenceData)SequenceData).VisibleTimeStart; }
			set { ((TimedSequenceData)SequenceData).VisibleTimeStart = value; }
		}

		public int DefaultRowHeight
		{
			get { return ((TimedSequenceData)SequenceData).DefaultRowHeight; }
			set { ((TimedSequenceData)SequenceData).DefaultRowHeight = value; }
		}

		public int DefaultSplitterDistance
		{
			get { return ((TimedSequenceData)SequenceData).DefaultSplitterDistance; }
			set { ((TimedSequenceData)SequenceData).DefaultSplitterDistance = value; }
		}

		public TimeSpan? DefaultPlaybackStartTime
		{
			get { return ((TimedSequenceData)SequenceData).DefaultPlaybackStartTime; }
			set { ((TimedSequenceData)SequenceData).DefaultPlaybackStartTime = value; }
		}

		public TimeSpan? DefaultPlaybackEndTime
		{
			get { return ((TimedSequenceData)SequenceData).DefaultPlaybackEndTime; }
			set { ((TimedSequenceData)SequenceData).DefaultPlaybackEndTime = value; }
		}

		public SequenceLayers SequenceLayers
		{
			get { return ((TimedSequenceData)SequenceData).SequenceLayers; }
			set { ((TimedSequenceData)SequenceData).SequenceLayers = value; }
		}

	}
}