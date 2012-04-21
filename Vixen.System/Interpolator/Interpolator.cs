using System;

namespace Vixen.Interpolator {
	public abstract class Interpolator<T> {
		public bool Interpolate(TimeSpan timeOffset, TimeSpan timeSpan, T startValue, T endValue, out T value) {
			float percent = (float)(timeOffset.TotalMilliseconds / timeSpan.TotalMilliseconds);
			return Interpolate(percent, startValue, endValue, out value);
		}

		public bool Interpolate(double percentage, T startValue, T endValue, out T value) {
			value = default(T);

			if(percentage > 0 && percentage < 1) {
				value = InterpolateValue(percentage, startValue, endValue);
				return true;
			}

			return false;
		}

		abstract protected T InterpolateValue(double percent, T startValue, T endValue);
	}
}
