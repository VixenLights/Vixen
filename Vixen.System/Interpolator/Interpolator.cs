using System;

namespace Vixen.Interpolator {
	public abstract class Interpolator<T> {
		public bool Interpolate(TimeSpan timeOffset, TimeSpan timeSpan, T startValue, T endValue, out T value) {
			value = default(T);

			if(timeOffset > TimeSpan.Zero && timeOffset < timeSpan) {
				float percent = (float)(timeOffset.TotalMilliseconds / timeSpan.TotalMilliseconds);
				value = InterpolateValue(startValue, endValue, percent);
				return true;
			}
			
			return false;
		}

		abstract protected T InterpolateValue(T startValue, T endValue, float percent);
	}
}
