using System;
using Vixen.Sys;
using Vixen.Data.Value;

namespace Vixen.Intent
{

	public class StaticLightingArrayIntentState : Dispatchable<StaticLightingArrayIntentState>, IIntentState
	{
		public StaticLightingArrayIntentState(StaticLightingArrayIntent intent, TimeSpan intentRelativeTime)
		{
			if (intent == null) throw new ArgumentNullException("intent");

			Intent = intent;
			RelativeTime = intentRelativeTime;
		}

		public StaticLightingArrayIntent Intent { get; private set; }

		IIntent IIntentState.Intent
		{
			get { return Intent; }
		}

		public TimeSpan RelativeTime { get; private set; }

		public LightingValue GetValue()
		{
			return Intent.GetStateAt(RelativeTime);
		}

		object IIntentState.GetValue()
		{
			return GetValue();
		}

		public IIntentState Clone()
		{
			StaticLightingArrayIntentState newIntentState = new StaticLightingArrayIntentState(Intent, RelativeTime);

			return newIntentState;
		}
	}





}
