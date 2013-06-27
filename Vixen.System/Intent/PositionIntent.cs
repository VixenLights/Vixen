using System;
using Vixen.Data.Value;

namespace Vixen.Intent
{
	public class PositionIntent : LinearIntent<PositionValue>
	{
		public PositionIntent(PositionValue startValue, PositionValue endValue, TimeSpan timeSpan)
			: base(startValue, endValue, timeSpan)
		{
		}
	}
}