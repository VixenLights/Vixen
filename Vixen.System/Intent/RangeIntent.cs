using System;
using Vixen.Data.Value;

namespace Vixen.Intent
{
	public class RangeIntent : NonSegmentedLinearIntent<RangeValue>
	{
		public RangeIntent(RangeValue startValue, RangeValue endValue, TimeSpan timeSpan)
			: base(startValue, endValue, timeSpan)
		{
		}
	}
}