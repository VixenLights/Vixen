using System;
using Vixen.Data.Value;

namespace Vixen.Intent
{
	public class RangeIntent : NonSegmentedLinearIntent<RangeValue<FunctionIdentity>>
	{
		public RangeIntent(RangeValue<FunctionIdentity> startValue, RangeValue<FunctionIdentity> endValue, TimeSpan timeSpan)
			: base(startValue, endValue, timeSpan)
		{
		}
	}
}