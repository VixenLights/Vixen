using System;
using Vixen.Sys;
using Vixen.Data.Value;
using Vixen.Sys.LayerMixing;

namespace Vixen.Intent
{

	public class StaticArrayIntent<T> : Dispatchable<StaticArrayIntent<T>>, IIntent<T>
		where T : IIntentDataType
	{
		private readonly T[] _values;
		readonly TimeSpan _frameTime;
		private readonly StaticIntentState<T> _intentState;

		public StaticArrayIntent(TimeSpan frameTime, T[] values, TimeSpan timeSpan)
		{
			if (values.Length == 0)
			{
				throw new ArgumentOutOfRangeException("values");
			}
			TimeSpan = timeSpan;
			_values = values;
			_frameTime = frameTime;
			_intentState = new StaticIntentState<T>(values[0]);
		}

		public TimeSpan TimeSpan { get; }

		public IIntentState CreateIntentState(TimeSpan intentRelativeTime, ILayer layer)
		{
			_intentState.Layer = layer;
			_intentState.SetValue(GetStateAt(intentRelativeTime));
			return _intentState;
		}

		object IIntent.GetStateAt(TimeSpan intentRelativeTime)
		{
			return GetStateAt(intentRelativeTime);
		}

		public T GetStateAt(TimeSpan intentRelativeTime)
		{
			int idx = (int)(intentRelativeTime.TotalMilliseconds / _frameTime.TotalMilliseconds);
			if (idx < 0)
				idx = 0;
			else if (idx >= _values.Length)
				idx = _values.Length - 1;
			//Console.WriteLine( "gsa: idx: {0}, rel: {1}, ft: {2}", idx, intentRelativeTime.TotalMilliseconds, _frameTime.TotalMilliseconds);
			return _values[idx];
		}

	}

}
