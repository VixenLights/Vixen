<<<<<<< HEAD
﻿namespace Vixen.Interpolator
{
	internal class StaticValueInterpolator<T> : Interpolator<T>
	{
		protected override T InterpolateValue(float percent, T startValue, T endValue)
		{
			return startValue;
		}
	}
=======
﻿namespace Vixen.Interpolator
{
	internal class StaticValueInterpolator<T> : Interpolator<T>
	{
		protected override T InterpolateValue(float percent, T startValue, T endValue)
		{
			return startValue;
		}
	}
>>>>>>> parent of 42f78e6... Git insists these need committing even tho nothing has changed
}