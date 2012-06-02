using System;
using System.Collections.Generic;
using System.Linq;
using Vixen.Intent;
using Vixen.Interpolator;

namespace Vixen.Sys {
	abstract public class LinearIntent<IntentType, TypeOfValue> : Dispatchable<IntentType>, IIntent<TypeOfValue>
		where IntentType : Dispatchable<IntentType> {
		private SortedList<TimeSpan, IntentSegmentNode<TypeOfValue>> _segmentTimeIndex;
		//An interval tree would be great, but I could only find one C# implementation on the web
		//and they're beyond me to implement.
		//private IntervalTree<IntentSegmentNode<TypeOfValue>, TimeSpan> _segmentIntervalIndex;
		private Interpolator<TypeOfValue> _interpolator;

		protected LinearIntent(TypeOfValue startValue, TypeOfValue endValue, TimeSpan timeSpan, Interpolator<TypeOfValue> interpolator = null) {
			_segmentTimeIndex = new SortedList<TimeSpan, IntentSegmentNode<TypeOfValue>>();
			StartValue = startValue;
			EndValue = endValue;
			TimeSpan = timeSpan;
			_interpolator = interpolator ?? Interpolator.Interpolator.Create<TypeOfValue>();
			IIntentSegment<TypeOfValue> initialSegment = _CreateSegment(startValue, endValue, timeSpan);
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

		object IIntent.GetStateAt(TimeSpan intentRelativeTime) {
			return GetStateAt(intentRelativeTime);
		}

		public TypeOfValue StartValue { get; private set; }

		public TypeOfValue EndValue { get; private set; }

		public TimeSpan TimeSpan { get; private set; }

		public void FractureAt(TimeSpan intentRelativeTime) {
			if(_IsValidTime(intentRelativeTime)) {
				IntentSegmentNode<TypeOfValue> segmentNode = _GetSegmentIntersecting(intentRelativeTime);
				if(segmentNode == null) return;
				TimeSpan segmentRelativeTime = _GetSegmentRelativeTime(segmentNode, intentRelativeTime);
				TypeOfValue valueAtTimePoint = segmentNode.Segment.GetStateAt(segmentRelativeTime);
				IIntentSegment<TypeOfValue> leftSegment = _CreateSegment(segmentNode.Segment.StartValue, valueAtTimePoint, segmentRelativeTime);
				IIntentSegment<TypeOfValue> rightSegment = _CreateSegment(valueAtTimePoint, segmentNode.Segment.EndValue, segmentNode.Segment.TimeSpan - segmentRelativeTime);
				if(leftSegment != null && rightSegment != null) {
					_RemoveAt(segmentNode.StartTime);
					_Insert(leftSegment, segmentNode.StartTime);
					_Insert(rightSegment, segmentNode.StartTime + segmentRelativeTime);
				}
			}
		}

		public void FractureAt(IEnumerable<TimeSpan> intentRelativeTimes) {
			foreach(TimeSpan intentRelativeTime in intentRelativeTimes) {
				FractureAt(intentRelativeTime);
			}
		}

		public void SplitAt(params TimeSpan[] intentRelativeTimes) {
			FractureAt(intentRelativeTimes);
		}

		public void FractureAt(ITimeNode intentRelativeTime) {
			SplitAt(intentRelativeTime.StartTime, intentRelativeTime.EndTime);
		}

		public IIntent[] DivideAt(TimeSpan intentRelativeTime) {
			if(!_IsValidTime(intentRelativeTime)) return null;

			FractureAt(intentRelativeTime);
			// Get all segments with an end time <= intentRelativeTime for the left intent.
			IEnumerable<IntentSegmentNode<TypeOfValue>> leftSegments = _segmentTimeIndex.Values.TakeWhile(x => x.EndTime <= intentRelativeTime);
			// Get all segments with a start time >= intentRelativeTime for the right intent.
			IEnumerable<IntentSegmentNode<TypeOfValue>> rightSegments = _segmentTimeIndex.Values.SkipWhile(x => x.EndTime <= intentRelativeTime);
			// Get the value at the point of divide.
			TypeOfValue dividePointValue = GetStateAt(intentRelativeTime);
			// Create the new intents.
			LinearIntent<IntentType, TypeOfValue>[] newIntents = new[] {
				Spawn(StartValue, dividePointValue, intentRelativeTime),
				Spawn(dividePointValue, EndValue, TimeSpan - intentRelativeTime)
			};
			// Distribute the segments appropriately.
			newIntents[0]._SetSegments(leftSegments);
			newIntents[1]._SetSegments(rightSegments);

			return newIntents;
		}

		public void ApplyFilter(ISequenceFilterNode sequenceFilterNode, TimeSpan contextAbsoluteIntentStartTime) {
			FractureAt(sequenceFilterNode.StartTime - contextAbsoluteIntentStartTime);
			FractureAt(sequenceFilterNode.EndTime - contextAbsoluteIntentStartTime);
			var segmentNodes = _GetIntersectingSegments(sequenceFilterNode);
			foreach(var segmentNode in segmentNodes) {
				sequenceFilterNode.AffectIntent(segmentNode.Segment, contextAbsoluteIntentStartTime + segmentNode.StartTime, contextAbsoluteIntentStartTime + segmentNode.EndTime);
			}
		}

		public IIntentState CreateIntentState(TimeSpan intentRelativeTime) {
			return new IntentState<TypeOfValue>(this, intentRelativeTime);
		}

		abstract protected LinearIntent<IntentType, TypeOfValue> Spawn(TypeOfValue startValue, TypeOfValue endValue, TimeSpan timeSpan);

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
			//Linear search. Just ugly.
			return _segmentTimeIndex.Values.FirstOrDefault(x => TimeNode.IntersectsExclusively(x, intentRelativeTime));
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

		private IIntentSegment<TypeOfValue> _CreateSegment(TypeOfValue startValue, TypeOfValue endValue, TimeSpan timeSpan) {
			return new IntentSegment<TypeOfValue>(startValue, endValue, timeSpan, _interpolator);
		}

		private void _Insert(IIntentSegment<TypeOfValue> segment, TimeSpan startTime) {
			IntentSegmentNode<TypeOfValue> segmentNode = new IntentSegmentNode<TypeOfValue>(segment, startTime);
			_segmentTimeIndex.Add(startTime, segmentNode);
		}

		private void _RemoveAt(TimeSpan segmentStartTime) {
			_segmentTimeIndex.Remove(segmentStartTime);
		}

		private bool _IsValidTime(TimeSpan intentRelativeTime) {
			return intentRelativeTime < TimeSpan && intentRelativeTime > TimeSpan.Zero;
		}

		private void _SetSegments(IEnumerable<IntentSegmentNode<TypeOfValue>> segmentNodes) {
			_segmentTimeIndex.Clear();
			foreach(IntentSegmentNode<TypeOfValue> segmentNode in segmentNodes) {
				_Insert(segmentNode.Segment, segmentNode.StartTime);
			}
		}
	}
}
