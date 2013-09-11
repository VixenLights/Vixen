using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Data.Value;

namespace Vixen.Interpolator
{
	[Vixen.Sys.Attribute.Interpolator(typeof(CustomValue))]
	internal class DynamicValueInterpolator :Interpolator<CustomValue>
	{
		protected override CustomValue InterpolateValue(double percent, CustomValue startValue, CustomValue endValue)
		{
			return new CustomValue(startValue);
		}
	}
}
