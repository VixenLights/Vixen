using System;
using Vixen.Data.Value;

namespace Vixen.Intent
{
	public class RGBIntent : NonSegmentedLinearIntent<RGBValue>
	{
		public RGBIntent(RGBValue startValue, RGBValue endValue, TimeSpan timeSpan)
			: base(startValue, endValue, timeSpan)
		{
		}
	}
}