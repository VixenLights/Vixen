using System;
using System.Collections.Generic;

namespace VixenModules.Sequence.Timed
{
	public class TimedSequence : BaseSequence.Sequence
	{
		public static string Extension = ".tim";

		public List<MarkCollection> MarkCollections
		{
			get { return ((TimedSequenceData) SequenceData).MarkCollections; }
			set { ((TimedSequenceData) SequenceData).MarkCollections = value; }
		}

		public List<RowHeightSetting> RowHeightSettings
		{
			get { return ((TimedSequenceData)SequenceData).RowHeightSettings; }
			set { ((TimedSequenceData)SequenceData).RowHeightSettings = value; }
		}

		public List<Guid> RowGuidId
		{
			get { return ((TimedSequenceData)SequenceData).RowGuidId; }
			set { ((TimedSequenceData)SequenceData).RowGuidId = value; }
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

	}
}