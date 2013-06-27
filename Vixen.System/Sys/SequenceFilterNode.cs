using System;
using Vixen.Module.SequenceFilter;

namespace Vixen.Sys
{
	public class SequenceFilterNode : ISequenceFilterNode
	{
		public SequenceFilterNode(ISequenceFilterModuleInstance filter, TimeSpan startTime)
		{
			Filter = filter;
			StartTime = startTime;
		}

		public ISequenceFilterModuleInstance Filter { get; private set; }

		public TimeSpan StartTime { get; private set; }

		public TimeSpan TimeSpan
		{
			get { return (Filter != null) ? Filter.TimeSpan : TimeSpan.Zero; }
		}

		public TimeSpan EndTime
		{
			get { return StartTime + TimeSpan; }
		}

		public void AffectIntent(IIntentSegment intentSegment, TimeSpan contextAbsoluteStartTime,
		                         TimeSpan contextAbsoluteEndTime)
		{
			TimeSpan filterRelativeStartTime = _GetFilterRelativeStartTime(contextAbsoluteStartTime);
			TimeSpan filterRelativeEndTime = _GetFilterRelativeEndTime(contextAbsoluteEndTime);
			if (filterRelativeStartTime < TimeSpan && filterRelativeEndTime > TimeSpan.Zero) {
				Filter.AffectIntent(intentSegment, filterRelativeStartTime, filterRelativeEndTime);
			}
		}

		private TimeSpan _GetFilterRelativeStartTime(TimeSpan contextAbsoluteStartTime)
		{
			double filterRelativeStartTime = _GetFilterRelativeTimeInMilliseconds(contextAbsoluteStartTime);
			return TimeSpan.FromMilliseconds(Math.Max(0, filterRelativeStartTime));
		}

		private TimeSpan _GetFilterRelativeEndTime(TimeSpan contextAbsoluteEndTime)
		{
			double filterRelativeEndTime = _GetFilterRelativeTimeInMilliseconds(contextAbsoluteEndTime);
			return TimeSpan.FromMilliseconds(Math.Min(Filter.TimeSpan.TotalMilliseconds, filterRelativeEndTime));
		}

		private double _GetFilterRelativeTimeInMilliseconds(TimeSpan time)
		{
			return (time - StartTime).TotalMilliseconds;
		}

		#region IComparable<ISequenceFilterNode>

		public int CompareTo(ISequenceFilterNode other)
		{
			return DataNode.Compare(this, other);
		}

		#endregion
	}

	public interface ISequenceFilterNode : IDataNode, IComparable<ISequenceFilterNode>
	{
		ISequenceFilterModuleInstance Filter { get; }
		void AffectIntent(IIntentSegment intentSegment, TimeSpan contextAbsoluteStartTime, TimeSpan contextAbsoluteEndTime);
	}
}