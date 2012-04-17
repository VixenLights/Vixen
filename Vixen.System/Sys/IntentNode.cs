using System;
using System.Collections.Generic;
using System.Linq;

namespace Vixen.Sys {
	public class IntentNode : IIntentNode, IComparer<IntentNode> {
		private SortedList<TimeNode,IntentNodeSegment> _segments;

		public IntentNode(IIntent intent, TimeSpan startTime) {
			Intent = intent;
			
			// Intended to be immutable.  Timing bounds for the segments.
			TimeNode = new TimeNode(startTime, intent.TimeSpan);

			_segments = new SortedList<TimeNode, IntentNodeSegment>();
			IntentNodeSegment segment = new IntentNodeSegment(intent, TimeNode);
			_segments.Add(segment.TimeNode, segment);

			SubordinateIntents = new List<SubordinateIntent>();
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

		virtual public IIntentState CreateIntentState(TimeSpan intentRelativeTime) {
			IIntentState intentState = Intent.CreateIntentState(intentRelativeTime);

			intentState.SubordinateIntentStates.AddRange(_GetSubordinateIntentStates(intentRelativeTime));

			return intentState;
		}

		public void SplitAt(TimeSpan intentRelativeTime) {
			IntentNodeSegment segment = _FindSegmentAt(intentRelativeTime);
			TimeSpan segmentRelativeTime = _GetSegmentRelativeTime(segment, intentRelativeTime);
			_SplitSegment(segment, segmentRelativeTime);
		}

		public void SplitAt(IEnumerable<TimeSpan> absoluteTimes) {
			foreach(TimeSpan absoluteTime in absoluteTimes) {
				SplitAt(absoluteTime);
			}
		}

		public void SplitAt(params TimeSpan[] absoluteTimes) {
			SplitAt((IEnumerable<TimeSpan>)absoluteTimes);
		}

		public void SplitAt(TimeNode absoluteTimeNode) {
			SplitAt(absoluteTimeNode.StartTime, absoluteTimeNode.EndTime);
		}

		private TimeSpan _GetSegmentRelativeTime(IntentNodeSegment segment, TimeSpan intentRelativeTime) {
			return intentRelativeTime - segment.StartTime;
		}

		private IntentNodeSegment _FindSegmentAt(TimeSpan intentRelativeTime) {
			// There should be no breaks in the segments that make up this intent.
			IntentNodeSegment segment = _segments.Values.FirstOrDefault(x => x.TimeNode.Intersects(intentRelativeTime));
			if(segment == null) throw new Exception("There is a gap in the intent segment at " + intentRelativeTime);
			return segment;
		}

		private void _SplitSegment(IntentNodeSegment segment, TimeSpan segmentRelativeTime) {
			if(Intent != null && segmentRelativeTime < segment.TimeSpan) {
				IntentNodeSegment[] segments = segment.SplitAt(segmentRelativeTime);
				if(segments != null) {
					// Replace the segment with the two segments.
					_RemoveSegment(segment);
					_InsertSegments(segments);
				}
			}
		}

		private void _RemoveSegment(IntentNodeSegment segment) {
			_segments.Remove(segment.TimeNode);
		}

		private void _InsertSegments(IEnumerable<IntentNodeSegment> segments) {
			foreach(IntentNodeSegment segment in segments) {
				_segments.Add(segment.TimeNode, segment);
			}
		}

		private IEnumerable<SubordinateIntentState> _GetSubordinateIntentStates(TimeSpan intentRelativeTime) {
			return SubordinateIntents.Select(x => _GetSubordinateIntentState(intentRelativeTime, x));
		}

		private SubordinateIntentState _GetSubordinateIntentState(TimeSpan intentRelativeTime, SubordinateIntent subordinateIntent) {
			TimeSpan otherIntentRelativeTime = Helper.TranslateIntentRelativeTime(intentRelativeTime, this, subordinateIntent.IntentNode);
			IIntentState otherIntentState = subordinateIntent.IntentNode.CreateIntentState(otherIntentRelativeTime);
			SubordinateIntentState subordinateIntentState = new SubordinateIntentState(otherIntentState, subordinateIntent.CombinationOperation);
			return subordinateIntentState;
		}

		public List<SubordinateIntent> SubordinateIntents { get; private set; }

		//#region IComparer<IIntentNode>
		//public class Comparer : IComparer<IIntentNode> {
		//    public int Compare(IIntentNode x, IIntentNode y) {
		//        return x.StartTime.CompareTo(y.StartTime);
		//    }
		//}
		//#endregion

		#region IComparable<IIntentNode>
		public int CompareTo(IIntentNode other) {
			return CompareTo((IDataNode)other);
		}
		#endregion

		#region IComparer<IntentNode>
		public int Compare(IntentNode x, IntentNode y) {
			return x.StartTime.CompareTo(y.StartTime);
		}
		#endregion

		#region IComparable<IDataNode>
		public int CompareTo(IDataNode other) {
			return TimeNode.CompareTo(other.TimeNode);
		}
		#endregion
	}

	public interface IIntentNode : IDataNode, IComparable<IIntentNode> {
		IIntent Intent { get; }
		IIntentState CreateIntentState(TimeSpan intentRelativeTime);
		List<SubordinateIntent> SubordinateIntents { get; }
	}
}
