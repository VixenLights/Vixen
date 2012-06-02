using System;
using System.Collections.Generic;

namespace Vixen.Sys {
	class IntentSegmentNode<T> : IComparable<IntentSegmentNode<T>>, IComparer<IntentSegmentNode<T>>, ITimeNode {
		public IntentSegmentNode(IIntentSegment<T> segment, TimeSpan startTime) {
			Segment = segment;
			StartTime = startTime;
		}

		public IIntentSegment<T> Segment { get; private set; }

		public TimeSpan StartTime { get; private set; }

		public TimeSpan TimeSpan {
			get { return Segment.TimeSpan; }
		}

		public TimeSpan EndTime {
			get { return StartTime + TimeSpan; }
		}

		public int CompareTo(IntentSegmentNode<T> other) {
			return StartTime.CompareTo(other.StartTime);
		}

		public int Compare(IntentSegmentNode<T> x, IntentSegmentNode<T> y) {
			return x.CompareTo(y);
		}
	}
}
