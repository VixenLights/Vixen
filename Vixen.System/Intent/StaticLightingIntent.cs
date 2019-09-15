using System;
using Vixen.Data.Value;
using Vixen.Interpolator;

namespace Vixen.Intent
{
	public class StaticLightingIntent:NonSegmentedLinearIntent<LightingValue>
	{
		private static readonly StaticLightingValueInterpolator Interpolator = new StaticLightingValueInterpolator();

		/// <inheritdoc />
		public StaticLightingIntent(LightingValue value, TimeSpan timeSpan) : base(value,value, timeSpan, Interpolator)
		{
		}
	}
}
