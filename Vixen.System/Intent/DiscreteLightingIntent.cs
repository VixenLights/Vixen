using System;
using Vixen.Data.Value;
using Vixen.Interpolator;

namespace Vixen.Intent
{
	public class DiscreteLightingIntent : NonSegmentedLinearIntent<DiscreteValue>
	{
		public DiscreteLightingIntent(DiscreteValue startValue, DiscreteValue endValue, TimeSpan timeSpan, Interpolator<DiscreteValue> interpolator = null) : base(startValue, endValue, timeSpan, interpolator)
		{
		}
	}
}
