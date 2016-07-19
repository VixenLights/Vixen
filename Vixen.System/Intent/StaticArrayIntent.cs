using System;
using System.Collections.Generic;
using Vixen.Sys;
using Vixen.Data.Value;
using Vixen.Sys.LayerMixing;

namespace Vixen.Intent
{

	public class StaticArrayIntent<T> : Dispatchable<StaticArrayIntent<T>>, IIntent<T>
		where T : IIntentDataType
	{
		readonly TimeSpan _timespan;
		readonly T[] _vals;
		TimeSpan _frameTime;
		private readonly IntentState<T> _intentState;

		public StaticArrayIntent(TimeSpan frameTime, T[] vals, TimeSpan timeSpan)
		{
			if (vals.Length == 0)
			{
				throw new ArgumentOutOfRangeException("vals");
			}
			_timespan = timeSpan;
			_vals = vals;
			_frameTime = frameTime;
			_intentState = new IntentState<T>(this, TimeSpan.Zero);
		}

		public TimeSpan TimeSpan { get { return _timespan; } }

		public IIntentState CreateIntentState(TimeSpan intentRelativeTime, ILayer layer)
		{
			//return new IntentState<T>(this, intentRelativeTime, layer);
			_intentState.Layer = layer;
			_intentState.RelativeTime = intentRelativeTime;
			return _intentState;
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

		public T GetStateAt(TimeSpan intentRelativeTime)
		{
			int idx = Math.Min(_vals.Length - 1, (int)(intentRelativeTime.TotalMilliseconds / _frameTime.TotalMilliseconds));
			if (idx < 0)
				idx = 0;
			else if (idx >= _vals.Length)
				idx = _vals.Length - 1;
			//Console.WriteLine( "gsa: idx: {0}, rel: {1}, ft: {2}", idx, intentRelativeTime.TotalMilliseconds, _frameTime.TotalMilliseconds);
			return _vals[idx];
		}

	}

}
