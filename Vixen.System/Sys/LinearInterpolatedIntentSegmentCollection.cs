using System;
using System.Collections.Generic;
using System.Linq;
using Vixen.Data.Value;
using Vixen.Interpolator;

namespace Vixen.Sys
{
	internal class LinearInterpolatedIntentSegmentCollection<TypeOfValue>
		where TypeOfValue : IIntentDataType
	{
		private SortedList<TimeSpan, IntentSegmentNode<TypeOfValue>> _segmentTimeIndex;
		private ISegmentCreator<TypeOfValue> _segmentCreator;

		private LinearInterpolatedIntentSegmentCollection()
		{
			_segmentTimeIndex = new SortedList<TimeSpan, IntentSegmentNode<TypeOfValue>>(1);
		}

		private LinearInterpolatedIntentSegmentCollection(ISegmentCreator<TypeOfValue> segmentCreator)
			: this()
		{
			_segmentCreator = segmentCreator;
		}

		private LinearInterpolatedIntentSegmentCollection(IEnumerable<IntentSegmentNode<TypeOfValue>> segments,
		                                                  ISegmentCreator<TypeOfValue> segmentCreator)
			: this(segmentCreator)
		{
			foreach (var segment in segments) {
				_segmentTimeIndex.Add(segment.StartTime, segment);
			}
		}

		public LinearInterpolatedIntentSegmentCollection(TypeOfValue startValue, TypeOfValue endValue, TimeSpan timeSpan,
		                                                 Interpolator<TypeOfValue> interpolator)
			: this(new LinearInterpolatedSegmentCreator<TypeOfValue>(interpolator))
		{
			IIntentSegment<TypeOfValue> initialSegment = _CreateSegment(startValue, endValue, timeSpan);
			_Insert(initialSegment, TimeSpan.Zero);
		}

		public IIntentSegment<TypeOfValue> GetSegmentAt(TimeSpan intentRelativeTime)
		{
			IntentSegmentNode<TypeOfValue> segmentNode = _GetSegmentIntersecting(intentRelativeTime);
			if (segmentNode != null) {
				return segmentNode.Segment;
			}
			return null;
		}

		public TypeOfValue GetStateAt(TimeSpan intentRelativeTime)
		{
			IntentSegmentNode<TypeOfValue> segmentNode = _GetSegmentIntersecting(intentRelativeTime);
			if (segmentNode != null) {
				TimeSpan segmentRelativeTime = _GetSegmentRelativeTime(segmentNode, intentRelativeTime);
				return segmentNode.Segment.GetStateAt(segmentRelativeTime);
			}
			return default(TypeOfValue);
		}

		public void FractureAt(TimeSpan intentRelativeTime)
		{
			IntentSegmentNode<TypeOfValue> segmentNode = _GetSegmentIntersecting(intentRelativeTime);
			if (segmentNode == null) return;
			TimeSpan segmentRelativeTime = _GetSegmentRelativeTime(segmentNode, intentRelativeTime);
			TypeOfValue valueAtTimePoint = segmentNode.Segment.GetStateAt(segmentRelativeTime);
			IIntentSegment<TypeOfValue> leftSegment = _CreateSegment(segmentNode.Segment.StartValue, valueAtTimePoint,
			                                                         segmentRelativeTime);
			IIntentSegment<TypeOfValue> rightSegment = _CreateSegment(valueAtTimePoint, segmentNode.Segment.EndValue,
			                                                          segmentNode.Segment.TimeSpan - segmentRelativeTime);
			if (leftSegment != null && rightSegment != null) {
				_RemoveAt(segmentNode.StartTime);
				_Insert(leftSegment, segmentNode.StartTime);
				_Insert(rightSegment, segmentNode.StartTime + segmentRelativeTime);
			}
		}

		public void DivideAt(TimeSpan intentRelativeTime,
		                     out LinearInterpolatedIntentSegmentCollection<TypeOfValue> leftSegments,
		                     out LinearInterpolatedIntentSegmentCollection<TypeOfValue> rightSegments)
		{
			FractureAt(intentRelativeTime);
			// Get all segments with an end time <= intentRelativeTime for the left intent.
			leftSegments =
				new LinearInterpolatedIntentSegmentCollection<TypeOfValue>(
					_segmentTimeIndex.Values.TakeWhile(x => x.EndTime <= intentRelativeTime), _segmentCreator);
			// Get all segments with a start time >= intentRelativeTime for the right intent.
			rightSegments =
				new LinearInterpolatedIntentSegmentCollection<TypeOfValue>(
					_segmentTimeIndex.Values.SkipWhile(x => x.EndTime <= intentRelativeTime), _segmentCreator);
		}

		public void ApplyFilter(ISequenceFilterNode sequenceFilterNode, TimeSpan contextAbsoluteIntentStartTime)
		{
			FractureAt(sequenceFilterNode.StartTime - contextAbsoluteIntentStartTime);
			FractureAt(sequenceFilterNode.EndTime - contextAbsoluteIntentStartTime);
			var segmentNodes = _GetIntersectingSegments(sequenceFilterNode);
			foreach (var segmentNode in segmentNodes) {
				sequenceFilterNode.AffectIntent(segmentNode.Segment, contextAbsoluteIntentStartTime + segmentNode.StartTime,
				                                contextAbsoluteIntentStartTime + segmentNode.EndTime);
			}
		}

		private IEnumerable<IntentSegmentNode<TypeOfValue>> _GetIntersectingSegments(ITimeNode timeNode)
		{
			int segmentIndex = _FindSegmentAt(timeNode.StartTime);
			var intentSegmentNode = _GetSegmentAtIndex(segmentIndex);
			while (intentSegmentNode != null && intentSegmentNode.StartTime < timeNode.EndTime) {
				yield return intentSegmentNode;
				// There should be no gaps or overlap in the segments of an intent.
				segmentIndex++;
				intentSegmentNode = _GetSegmentAtIndex(segmentIndex);
			}
		}

		private TimeSpan _GetSegmentRelativeTime(IntentSegmentNode<TypeOfValue> segment, TimeSpan intentRelativeTime)
		{
			return intentRelativeTime - segment.StartTime;
		}

		private IntentSegmentNode<TypeOfValue> _GetSegmentIntersecting(TimeSpan intentRelativeTime)
		{
			//Linear search. Just ugly.
			//return _segmentTimeIndex.Values.FirstOrDefault(x => TimeNode.IntersectsExclusively(x, intentRelativeTime));
			//Trying to squeak a little bit more out of this by using a for loop instead of LINQ
			//since we have a concrete, non-deferred collection.
			for (int i = 0; i < _segmentTimeIndex.Count; i++) {
				IntentSegmentNode<TypeOfValue> segmentNode = _segmentTimeIndex[_segmentTimeIndex.Keys[i]];
				if (TimeNode.IntersectsExclusively(segmentNode, intentRelativeTime)) {
					return segmentNode;
				}
			}

			return null;
		}

		private IntentSegmentNode<TypeOfValue> _GetSegmentAtIndex(int index)
		{
			if (index < _segmentTimeIndex.Count) {
				TimeSpan segmentStartTime = _segmentTimeIndex.Keys[index];
				return _segmentTimeIndex[segmentStartTime];
			}
			return null;
		}

		private int _FindSegmentAt(TimeSpan intentRelativeTime)
		{
			return _segmentTimeIndex.IndexOfKey(intentRelativeTime);
		}

		private IIntentSegment<TypeOfValue> _CreateSegment(TypeOfValue startValue, TypeOfValue endValue, TimeSpan timeSpan)
		{
			return _segmentCreator.CreateSegment(startValue, endValue, timeSpan);
		}

		private void _Insert(IIntentSegment<TypeOfValue> segment, TimeSpan startTime)
		{
			IntentSegmentNode<TypeOfValue> segmentNode = new IntentSegmentNode<TypeOfValue>(segment, startTime);
			_segmentTimeIndex.Add(startTime, segmentNode);
		}

		private void _RemoveAt(TimeSpan segmentStartTime)
		{
			_segmentTimeIndex.Remove(segmentStartTime);
		}
	}
}