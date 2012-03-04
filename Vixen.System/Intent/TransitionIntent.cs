using System;
using Vixen.Interpolator;
using Vixen.Sys;

namespace Vixen.Intent {
	public abstract class TransitionIntent<T> : Dispatchable<TransitionIntent<T>>, IIntent<T>
		where T : struct {

		private Interpolator<T> _interpolator;

		protected TransitionIntent(T startValue, T endValue, TimeSpan timeSpan, Interpolator<T> interpolator) {
			StartValue = startValue;
			EndValue = endValue;
			TimeSpan = timeSpan;
			_interpolator = interpolator;
		}

		abstract public IIntentState CreateIntentState(TimeSpan intentRelativeTime);

		public T StartValue { get; private set; }

		public T EndValue { get; private set; }

		public TimeSpan TimeSpan { get; private set; }

		public T GetCurrentState(TimeSpan timeOffset) {
			T value;
			_interpolator.Interpolate(timeOffset, TimeSpan, StartValue, EndValue, out value);
			return value;
		}
	}
}
