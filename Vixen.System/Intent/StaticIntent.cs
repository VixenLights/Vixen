using System;
using System.Collections.Generic;
using Vixen.Data.Value;
using Vixen.Sys;
using Vixen.Sys.LayerMixing;

namespace Vixen.Intent
{
	public class StaticIntent<TypeOfValue> : Dispatchable<StaticIntent<TypeOfValue>>, IIntent<TypeOfValue>
		where TypeOfValue : IIntentDataType
	{
		public StaticIntent(TypeOfValue value, TimeSpan timeSpan)
		{
			Value = value;
			TimeSpan = timeSpan;
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

		public void FractureAt(TimeSpan intentRelativeTime)
		{
		}

		public void FractureAt(IEnumerable<TimeSpan> intentRelativeTimes)
		{
		}

		public void FractureAt(params TimeSpan[] intentRelativeTimes)
		{
		}

		public void FractureAt(ITimeNode intentRelativeTime)
		{
		}

		public IIntent[] DivideAt(TimeSpan intentRelativeTime)
		{
			if (!_IsValidTime(intentRelativeTime)) return null;

			// Create the new intents.
			StaticIntent<TypeOfValue> a = new StaticIntent<TypeOfValue>(Value, intentRelativeTime);
			StaticIntent<TypeOfValue> b = new StaticIntent<TypeOfValue>(Value, TimeSpan - intentRelativeTime);

			return new[] { a, b };
		}

		public void ApplyFilter(ISequenceFilterNode sequenceFilterNode, TimeSpan contextAbsoluteIntentStartTime)
		{
			throw new NotImplementedException();
		}

		public IIntentState CreateIntentState(TimeSpan intentRelativeTime, ILayer layer)
		{
			return new IntentState<TypeOfValue>(this, intentRelativeTime, layer);
		}

		private bool _IsValidTime(TimeSpan intentRelativeTime)
		{
			return intentRelativeTime < TimeSpan && intentRelativeTime > TimeSpan.Zero;
		}

	}
}
