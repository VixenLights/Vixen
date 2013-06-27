using System;
using Vixen.Data.Value;
using Vixen.Sys;

namespace Vixen.Intent
{
	public class LightingIntent : LinearIntent<LightingValue>
	{
		public LightingIntent(LightingValue startValue, LightingValue endValue, TimeSpan timeSpan)
			: base(startValue, endValue, timeSpan)
		{
		}
	}
}