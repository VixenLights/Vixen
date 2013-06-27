using System;
using System.Collections.Generic;
using Vixen.Data.Value;
using Vixen.Interpolator;
using Vixen.Sys;

namespace Vixen.Intent
{
	public class LinearIntent<TypeOfValue> : Dispatchable<LinearIntent<TypeOfValue>>, IIntent<TypeOfValue>
		where TypeOfValue : IIntentDataType
	{
		private LinearInterpolatedIntentSegmentCollection<TypeOfValue> _segments;

		public LinearIntent(TypeOfValue startValue, TypeOfValue endValue, TimeSpan timeSpan,
		                    Interpolator<TypeOfValue> interpolator = null)
			: this(startValue, endValue, timeSpan)
		{
			interpolator = interpolator ?? Interpolator.Interpolator.Create<TypeOfValue>();
			_segments = new LinearInterpolatedIntentSegmentCollection<TypeOfValue>(startValue, endValue, timeSpan, interpolator);
		}

		private LinearIntent(TypeOfValue startValue, TypeOfValue endValue, TimeSpan timeSpan)
		{
			StartValue = startValue;
			EndValue = endValue;
			TimeSpan = timeSpan;
		}

		private LinearIntent(TypeOfValue startValue, TypeOfValue endValue, TimeSpan timeSpan,
		                     LinearInterpolatedIntentSegmentCollection<TypeOfValue> segments)
			: this(startValue, endValue, timeSpan)
		{
			_segments = segments;
		}

		public TypeOfValue GetStateAt(TimeSpan intentRelativeTime)
		{
			return _segments.GetStateAt(intentRelativeTime);
		}

		object IIntent.GetStateAt(TimeSpan intentRelativeTime)
		{
			return GetStateAt(intentRelativeTime);
		}

		public TypeOfValue StartValue { get; private set; }

		public TypeOfValue EndValue { get; private set; }

		public TimeSpan TimeSpan { get; private set; }

		public void FractureAt(TimeSpan intentRelativeTime)
		{
			if (_IsValidTime(intentRelativeTime)) {
				_segments.FractureAt(intentRelativeTime);
			}
		}

		public void FractureAt(IEnumerable<TimeSpan> intentRelativeTimes)
		{
			foreach (TimeSpan intentRelativeTime in intentRelativeTimes) {
				FractureAt(intentRelativeTime);
			}
		}

		public void FractureAt(params TimeSpan[] intentRelativeTimes)
		{
			FractureAt((IEnumerable<TimeSpan>) intentRelativeTimes);
		}

		public void FractureAt(ITimeNode intentRelativeTime)
		{
			FractureAt(intentRelativeTime.StartTime, intentRelativeTime.EndTime);
		}

		public IIntent[] DivideAt(TimeSpan intentRelativeTime)
		{
			if (!_IsValidTime(intentRelativeTime)) return null;

			// Divide the segments at the time point.
			LinearInterpolatedIntentSegmentCollection<TypeOfValue> leftSegments;
			LinearInterpolatedIntentSegmentCollection<TypeOfValue> rightSegments;
			_segments.DivideAt(intentRelativeTime, out leftSegments, out rightSegments);

			// Get the value at the point of divide.
			TypeOfValue dividePointValue = GetStateAt(intentRelativeTime);

			// Create the new intents.
			LinearIntent<TypeOfValue> leftIntent = new LinearIntent<TypeOfValue>(StartValue, dividePointValue, intentRelativeTime,
			                                                                     leftSegments);
			LinearIntent<TypeOfValue> rightIntent = new LinearIntent<TypeOfValue>(dividePointValue, EndValue,
			                                                                      TimeSpan - intentRelativeTime, rightSegments);

			return new[] {leftIntent, rightIntent};
		}

		public void ApplyFilter(ISequenceFilterNode sequenceFilterNode, TimeSpan contextAbsoluteIntentStartTime)
		{
			_segments.ApplyFilter(sequenceFilterNode, contextAbsoluteIntentStartTime);
		}

		public IIntentState CreateIntentState(TimeSpan intentRelativeTime)
		{
			return new IntentState<TypeOfValue>(this, intentRelativeTime);
		}

		private bool _IsValidTime(TimeSpan intentRelativeTime)
		{
			return intentRelativeTime < TimeSpan && intentRelativeTime > TimeSpan.Zero;
		}
	}
}