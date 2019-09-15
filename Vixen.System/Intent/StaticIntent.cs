using System;
using Vixen.Data.Value;
using Vixen.Sys;
using Vixen.Sys.LayerMixing;

namespace Vixen.Intent
{
	public class StaticIntent<TypeOfValue> : Dispatchable<StaticIntent<TypeOfValue>>, IIntent<TypeOfValue>
		where TypeOfValue : IIntentDataType
	{
		private readonly StaticIntentState<TypeOfValue> _intentState;

		public StaticIntent(TypeOfValue value, TimeSpan timeSpan)
		{
			Value = value;
			TimeSpan = timeSpan;
			_intentState = new StaticIntentState<TypeOfValue>(value);
		}

		public TypeOfValue Value { get; private set; }

		public TimeSpan TimeSpan { get; private set; }


		public TypeOfValue GetStateAt(TimeSpan intentRelativeTime)
		{
			return Value;
		}

		object IIntent.GetStateAt(TimeSpan intentRelativeTime)
		{
			return GetStateAt(intentRelativeTime);
		}

		public IIntentState CreateIntentState(TimeSpan intentRelativeTime, ILayer layer)
		{
			_intentState.Layer = layer;
			return _intentState;
		}
	}
}
