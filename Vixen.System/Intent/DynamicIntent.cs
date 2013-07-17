using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Data.Value;
using Vixen.Interpolator;

namespace Vixen.Intent
{
	public class DynamicIntent : LinearIntent<DynamicValue>
	{
		public DynamicIntent(DynamicValue value, TimeSpan timespan):
			base(value,value,timespan, new DynamicValueInterpolator())
		{
		}
	}
}
