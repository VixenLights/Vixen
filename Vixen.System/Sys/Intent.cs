using System;
using System.Collections.Generic;
using System.Linq;
using Vixen.Interpolator;
using IntervalTreeLib;

namespace Vixen.Sys {
	// All intents are now to be linear interpolations.
	abstract public class Intent<IntentType,TypeOfValue> : Dispatchable<IntentType>, IIntent<TypeOfValue>
		where IntentType : Dispatchable<IntentType> {
		private SortedList<TimeSpan, IntentSegmentNode<TypeOfValue>> _segmentTimeIndex;
		private IntervalTree<IntentSegmentNode<TypeOfValue>, TimeSpan> _segmentIntervalIndex;
		private Interpolator<TypeOfValue> _interpolator;

		protected Intent(TypeOfValue startValue, TypeOfValue endValue, TimeSpan timeSpan, Interpolator<TypeOfValue> interpolator) {
			_segmentTimeIndex = new SortedList<TimeSpan, IntentSegmentNode<TypeOfValue>>();
			_segmentIntervalIndex = new IntervalTree<IntentSegmentNode<TypeOfValue>, TimeSpan>();
			StartValue = startValue;
			EndValue = endValue;
			TimeSpan = timeSpan;
			_interpolator = interpolator;
			IntentSegment<TypeOfValue> initialSegment = _CreateSegment(startValue, endValue, timeSpan);
			_Insert(initialSegment, TimeSpan.Zero);
		}

		public TypeOfValue GetStateAt(TimeSpan intentRelativeTime) {
			IntentSegmentNode<TypeOfValue> segmentNode = _GetSegmentIntersecting(intentRelativeTime);
			if(segmentNode != null) {
				TimeSpan segmentRelativeTime = _GetSegmentRelativeTime(segmentNode, intentRelativeTime);
				return segmentNode.Segment.GetStateAt(segmentRelativeTime);
			}
			return default(TypeOfValue);
		}

		public TypeOfValue StartValue { get; private set; }

		public TypeOfValue EndValue { get; private set; }

		public TimeSpan TimeSpan { get; private set; }

		public void SplitAt(TimeSpan intentRelativeTime) {
			if(intentRelativeTime < TimeSpan && intentRelativeTime > TimeSpan.Zero) {
				IntentSegmentNode<TypeOfValue> segmentNode = _GetSegmentIntersecting(intentRelativeTime);
				if(segmentNode == null) return;
				TimeSpan segmentRelativeTime = _GetSegmentRelativeTime(segmentNode, intentRelativeTime);
				TypeOfValue valueAtTimePoint = segmentNode.Segment.GetStateAt(segmentRelativeTime);
				IntentSegment<TypeOfValue> leftSegment = _CreateSegment(segmentNode.Segment.StartValue, valueAtTimePoint, segmentRelativeTime);
				IntentSegment<TypeOfValue> rightSegment = _CreateSegment(valueAtTimePoint, segmentNode.Segment.EndValue, segmentNode.Segment.TimeSpan - segmentRelativeTime);
				if(leftSegment != null && rightSegment != null) {
					_RemoveAt(segmentNode.StartTime);
					_Insert(leftSegment, segmentNode.StartTime);
					_Insert(rightSegment, segmentNode.StartTime + segmentRelativeTime);
				}
			}
		}

		public void SplitAt(IEnumerable<TimeSpan> intentRelativeTimes) {
			foreach(TimeSpan intentRelativeTime in intentRelativeTimes) {
				SplitAt(intentRelativeTime);
			}
		}

		public void SplitAt(params TimeSpan[] intentRelativeTimes) {
			SplitAt((IEnumerable<TimeSpan>)intentRelativeTimes);
		}

		public void SplitAt(ITimeNode intentRelativeTime) {
			SplitAt(intentRelativeTime.StartTime, intentRelativeTime.EndTime);
		}

		public void ApplyFilter(IPreFilterNode preFilterNode, TimeSpan contextAbsoluteIntentStartTime) {
			SplitAt(preFilterNode);
			var segmentNodes = _GetIntersectingSegments(preFilterNode);
			foreach(var segmentNode in segmentNodes) {
				preFilterNode.AffectIntent(segmentNode.Segment, contextAbsoluteIntentStartTime + segmentNode.StartTime, contextAbsoluteIntentStartTime + segmentNode.EndTime);
			}
		}

		abstract public IIntentState CreateIntentState(TimeSpan intentRelativeTime);

		private IEnumerable<IntentSegmentNode<TypeOfValue>> _GetIntersectingSegments(ITimeNode timeNode) {
			int segmentIndex = _FindSegmentAt(timeNode.StartTime);
			var intentSegmentNode = _GetSegmentAtIndex(segmentIndex);
			while(intentSegmentNode != null && intentSegmentNode.StartTime < timeNode.EndTime) {
				yield return intentSegmentNode;
				// There should be no gaps or overlap in the segments of an intent.
				segmentIndex++;
				intentSegmentNode = _GetSegmentAtIndex(segmentIndex);
			}
		}

		private TimeSpan _GetSegmentRelativeTime(IntentSegmentNode<TypeOfValue> segment, TimeSpan intentRelativeTime) {
			return intentRelativeTime - segment.StartTime;
		}

		private IntentSegmentNode<TypeOfValue> _GetSegmentIntersecting(TimeSpan intentRelativeTime) {
			return _segmentIntervalIndex.Get(intentRelativeTime).FirstOrDefault();
		}

		private IntentSegmentNode<TypeOfValue> _GetSegmentExactlyAt(TimeSpan intentRelativeTime) {
			IntentSegmentNode<TypeOfValue> segmentNode;
			_segmentTimeIndex.TryGetValue(intentRelativeTime, out segmentNode);
			return segmentNode;
		}

		private IntentSegmentNode<TypeOfValue> _GetSegmentAtIndex(int index) {
			if(index < _segmentTimeIndex.Count) {
				TimeSpan segmentStartTime = _segmentTimeIndex.Keys[index];
				return _segmentTimeIndex[segmentStartTime];
			}
			return null;
		}

		private int _FindSegmentAt(TimeSpan intentRelativeTime) {
			return _segmentTimeIndex.IndexOfKey(intentRelativeTime);
		}

		private IntentSegment<TypeOfValue> _CreateSegment(TypeOfValue startValue, TypeOfValue endValue, TimeSpan timeSpan) {
			return new IntentSegment<TypeOfValue>(startValue, endValue, timeSpan, _interpolator);
		}

		private void _Insert(IntentSegment<TypeOfValue> segment, TimeSpan startTime) {
			IntentSegmentNode<TypeOfValue> segmentNode = new IntentSegmentNode<TypeOfValue>(segment, startTime);
			_segmentTimeIndex.Add(startTime, segmentNode);
			_segmentIntervalIndex.AddInterval(startTime, segment.TimeSpan, segmentNode);
		}

		private void _RemoveAt(TimeSpan segmentStartTime) {
			_segmentTimeIndex.Remove(segmentStartTime);

			var interval = _segmentIntervalIndex.GetIntervals(segmentStartTime).FirstOrDefault();
			if(interval != null) {
				_segmentIntervalIndex.Intervals.Remove(interval);
			}
		}
	}
}
