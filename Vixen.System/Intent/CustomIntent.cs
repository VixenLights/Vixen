using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Commands;
using Vixen.Data.Value;
using Vixen.Interpolator;

namespace Vixen.Intent
{
	public class CustomIntent : LinearIntent<CustomValue>
	{
		public CustomIntent(CustomValue value, TimeSpan timespan):
			base(value, value, timespan, new StaticValueInterpolator<CustomValue>())
		{
		}
	}
}
