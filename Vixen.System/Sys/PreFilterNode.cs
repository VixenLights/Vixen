using System;
using System.Diagnostics;
using Vixen.Module.PreFilter;

namespace Vixen.Sys {
	public class PreFilterNode : IPreFilterNode {
		public PreFilterNode(IPreFilterModuleInstance preFilter, TimeSpan startTime) {
			PreFilter = preFilter;
			StartTime = startTime;
		}

		public IPreFilterModuleInstance PreFilter { get; private set; }

		public TimeSpan StartTime { get; private set; }

		public TimeSpan TimeSpan {
			get { return (PreFilter != null) ? PreFilter.TimeSpan : TimeSpan.Zero; }
		}

		public TimeSpan EndTime {
			get { return StartTime + TimeSpan; }
		}

		public void AffectIntent(IIntentSegment intentSegment, TimeSpan contextAbsoluteStartTime, TimeSpan contextAbsoluteEndTime) {
			TimeSpan filterRelativeStartTime = _GetFilterRelativeStartTime(contextAbsoluteStartTime);
			TimeSpan filterRelativeEndTime = _GetFilterRelativeEndTime(contextAbsoluteEndTime);
			if(filterRelativeStartTime < TimeSpan && filterRelativeEndTime > TimeSpan.Zero) {
				PreFilter.AffectIntent(intentSegment, filterRelativeStartTime, filterRelativeEndTime);
			}
		}

		private TimeSpan _GetFilterRelativeStartTime(TimeSpan contextAbsoluteStartTime) {
			double filterRelativeStartTime = _GetFilterRelativeTimeInMilliseconds(contextAbsoluteStartTime);
			return TimeSpan.FromMilliseconds(Math.Max(0, filterRelativeStartTime));
		}

		private TimeSpan _GetFilterRelativeEndTime(TimeSpan contextAbsoluteEndTime) {
			double filterRelativeEndTime = _GetFilterRelativeTimeInMilliseconds(contextAbsoluteEndTime);
			return TimeSpan.FromMilliseconds(Math.Min(PreFilter.TimeSpan.TotalMilliseconds, filterRelativeEndTime));
		}

		private double _GetFilterRelativeTimeInMilliseconds(TimeSpan time) {
			return (time - StartTime).TotalMilliseconds;
		}

		#region IComparable<IPreFilterNode>
		public int CompareTo(IPreFilterNode other) {
			return DataNode.Compare(this, other);
		}
		#endregion
	}

	public interface IPreFilterNode : IDataNode, IComparable<IPreFilterNode> {
		IPreFilterModuleInstance PreFilter { get; }
		void AffectIntent(IIntentSegment intentSegment, TimeSpan contextAbsoluteStartTime, TimeSpan contextAbsoluteEndTime);
	}
}
