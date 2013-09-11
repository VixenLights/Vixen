using System;
using System.Collections.Generic;
using Vixen.Sys;
using Vixen.Data.Value;

namespace Vixen.Intent
{

	public class StaticLightingArrayIntent : Dispatchable<StaticLightingArrayIntent>, IIntent<LightingValue>
	{
		TimeSpan _timespan;
		LightingValue[] _vals;
		TimeSpan _frameTime;

		public StaticLightingArrayIntent( TimeSpan frameTime, LightingValue[] vals, TimeSpan timeSpan)
		{
			_timespan = timeSpan;
			_vals = vals;
			_frameTime = frameTime;
		}

		public TimeSpan TimeSpan { get { return _timespan; } private set { } }

		public IIntentState CreateIntentState(TimeSpan intentRelativeTime)
		{
			return new StaticLightingArrayIntentState(this, intentRelativeTime);
		}

		public void FractureAt(TimeSpan intentRelativeTime)
		{
		}

		public void FractureAt(IEnumerable<TimeSpan> intentRelativeTimes)
		{
		}

		public void FractureAt(ITimeNode intentRelativeTime)
		{
		}

		public IIntent[] DivideAt(TimeSpan intentRelativeTime)
		{
			if (intentRelativeTime >= _timespan || intentRelativeTime <= TimeSpan.Zero)
				return null;
			throw new NotImplementedException();
		}

		public void ApplyFilter(ISequenceFilterNode sequenceFilterNode, TimeSpan contextAbsoluteIntentStartTime)
		{
			throw new NotImplementedException();
		}

		object IIntent.GetStateAt(TimeSpan intentRelativeTime)
		{
			return GetStateAt(intentRelativeTime);
		}

		public LightingValue GetStateAt(TimeSpan intentRelativeTime)
		{
			int idx = Math.Min(_vals.Length - 1, (int) (intentRelativeTime.TotalMilliseconds / _frameTime.TotalMilliseconds) );
			if( idx < 0)
				idx = 0;
			else if( idx >= _vals.Length)
				idx = _vals.Length-1;
			return _vals[idx];
		}

	}





}
