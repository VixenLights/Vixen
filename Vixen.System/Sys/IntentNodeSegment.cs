using System;
using System.Collections.Generic;

namespace Vixen.Sys {
	// Times are relative to the start of the owning IntentNode.
	public class IntentNodeSegment : IDataNode, IComparer<IntentNodeSegment> {
		internal IntentNodeSegment(IIntent intent, TimeNode timeNode) {
			Intent = intent;
			TimeNode = timeNode;
		}

		public IIntent Intent { get; private set; }

		public TimeNode TimeNode { get; private set; }

		public TimeSpan StartTime {
			get { return TimeNode.StartTime; }
		}

		public TimeSpan TimeSpan {
			get { return TimeNode.TimeSpan; }
		}

		public TimeSpan EndTime {
			get { return TimeNode.EndTime; }
		}

		public IntentNodeSegment[] SplitAt(TimeSpan segmentRelativeTime) {
			if(Intent != null && TimeNode.Intersects(segmentRelativeTime)) {
				Intent.CreateSegment(StartTime, segmentRelativeTime - StartTime);
				Intent.CreateSegment(StartTime + segmentRelativeTime, TimeSpan - segmentRelativeTime);
			}
			// Can't split at this time point.
			return null;
		}

		public int CompareTo(IDataNode other) {
			return TimeNode.CompareTo(other.TimeNode);
		}

		public int Compare(IntentNodeSegment x, IntentNodeSegment y) {
			return x.CompareTo(y);
		}
	}
}
