using System;
using Vixen.Intents.Interpolators;
using Vixen.Sys;

namespace Vixen.Intents {
	//public abstract class TransitionIntent<T> : Dispatchable<TransitionIntent<T>>, IIntent
	//    where T : struct {

	//    private Interpolator<T> _interpolator;

	//    protected TransitionIntent(T startValue, T endValue, TimeSpan timeSpan, Interpolator<T> interpolator) {
	//        StartValue = startValue;
	//        EndValue = endValue;
	//        TimeSpan = timeSpan;
	//        _interpolator = interpolator;
	//    }

	//    public T StartValue { get; private set; }

	//    public T EndValue { get; private set; }

	//    public TimeSpan TimeSpan { get; private set; }

	//    public ICommand GetCurrentState(TimeSpan timeOffset) {
	//        T value;
	//        if(_interpolator.Interpolate(timeOffset, TimeSpan, StartValue, EndValue, out value)) {
	//            return GetCommandForValue(value);
	//        }
	//        return null;
	//    }

	//    abstract protected ICommand GetCommandForValue(T value);

	//    //// Must be done in the classes being dispatched.
	//    //abstract public void Dispatch(IntentDispatch intentDispatch);
	//}

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
