using System;
using Vixen.Data.Value;
using Vixen.Interpolator;

namespace Vixen.Intent
{
	public class StaticDiscreteLightingIntent: NonSegmentedLinearIntent<DiscreteValue>
	{
		private static readonly StaticDiscreteValueInterpolator Interpolator = new StaticDiscreteValueInterpolator();
		/// <inheritdoc />
		public StaticDiscreteLightingIntent(DiscreteValue value, TimeSpan timeSpan) : base(value,value, timeSpan, Interpolator)
		{
		}
	}
}
