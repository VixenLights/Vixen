using System;
using System.Collections.Generic;
using Vixen.Data.Value;
using Vixen.Interpolator;
using Vixen.Sys;
using Vixen.Sys.LayerMixing;

namespace Vixen.Intent
{
	public class SegmentedLinearIntent<TypeOfValue> : Dispatchable<SegmentedLinearIntent<TypeOfValue>>, IIntent<TypeOfValue>
		where TypeOfValue : IIntentDataType
	{
		private LinearInterpolatedIntentSegmentCollection<TypeOfValue> _segments;
		private readonly IntentState<TypeOfValue> _intentState;

		public SegmentedLinearIntent(TypeOfValue startValue, TypeOfValue endValue, TimeSpan timeSpan,
							Interpolator<TypeOfValue> interpolator = null)
			: this(startValue, endValue, timeSpan)
		{
			interpolator = interpolator ?? Interpolator.Interpolator.Create<TypeOfValue>();
			_segments = new LinearInterpolatedIntentSegmentCollection<TypeOfValue>(startValue, endValue, timeSpan, interpolator);
		}

		private SegmentedLinearIntent(TypeOfValue startValue, TypeOfValue endValue, TimeSpan timeSpan)
		{
			StartValue = startValue;
			EndValue = endValue;
			TimeSpan = timeSpan;
			_intentState = new IntentState<TypeOfValue>(this, TimeSpan.Zero);
		}

		private SegmentedLinearIntent(TypeOfValue startValue, TypeOfValue endValue, TimeSpan timeSpan,
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
			if (_IsValidTime(intentRelativeTime))
			{
				_segments.FractureAt(intentRelativeTime);
			}
		}

		public void FractureAt(IEnumerable<TimeSpan> intentRelativeTimes)
		{
			foreach (TimeSpan intentRelativeTime in intentRelativeTimes)
			{
				FractureAt(intentRelativeTime);
			}
		}

		public void FractureAt(params TimeSpan[] intentRelativeTimes)
		{
			FractureAt((IEnumerable<TimeSpan>)intentRelativeTimes);
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
			SegmentedLinearIntent<TypeOfValue> leftIntent = new SegmentedLinearIntent<TypeOfValue>(StartValue, dividePointValue, intentRelativeTime,
																				 leftSegments);
			SegmentedLinearIntent<TypeOfValue> rightIntent = new SegmentedLinearIntent<TypeOfValue>(dividePointValue, EndValue,
																				  TimeSpan - intentRelativeTime, rightSegments);

			return new[] { leftIntent, rightIntent };
		}

		public void ApplyFilter(ISequenceFilterNode sequenceFilterNode, TimeSpan contextAbsoluteIntentStartTime)
		{
			_segments.ApplyFilter(sequenceFilterNode, contextAbsoluteIntentStartTime);
		}

		public IIntentState CreateIntentState(TimeSpan intentRelativeTime, ILayer layer)
		{
			//return new IntentState<TypeOfValue>(this, intentRelativeTime, layer);
			_intentState.Layer = layer;
			_intentState.RelativeTime = intentRelativeTime;
			return _intentState;
		}

		private bool _IsValidTime(TimeSpan intentRelativeTime)
		{
			return intentRelativeTime < TimeSpan && intentRelativeTime > TimeSpan.Zero;
		}
	}
}