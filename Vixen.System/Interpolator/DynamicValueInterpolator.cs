using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Data.Value;

namespace Vixen.Interpolator
{
	[Vixen.Sys.Attribute.Interpolator(typeof(DynamicValue))]
	internal class DynamicValueInterpolator :Interpolator<DynamicValue>
	{
		protected override DynamicValue InterpolateValue(double percent, DynamicValue startValue, DynamicValue endValue)
		{
			return new DynamicValue(startValue);
		}
	}
}
