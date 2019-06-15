using System;
using Vixen.Data.Value;

namespace Vixen.Intent
{
	public class StaticLightingIntent:StaticIntent<LightingValue>
	{
		/// <inheritdoc />
		public StaticLightingIntent(LightingValue value, TimeSpan timeSpan) : base(value, timeSpan)
		{
		}
	}
}
