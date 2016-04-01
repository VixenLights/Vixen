using System;
using Vixen.Data.Value;

namespace Vixen.Intent
{
	public class LightingIntent : NonSegmentedLinearIntent<LightingValue>
	{
		public LightingIntent(LightingValue startValue, LightingValue endValue, TimeSpan timeSpan)
			: base(startValue, endValue, timeSpan)
		{
			
		}
		
	}
}