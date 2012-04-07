using System;
using System.Drawing;
using Vixen.Sys.Dispatch;

namespace Vixen.Sys.CombinationOperation {
	abstract class Combiner<T> : ICombiner<T>, IAnyIntentStateHandler {
		protected T Value;

		virtual public T Combine(T value, IIntentState intentState) {
			Value = value;
			intentState.Dispatch(this);
			return Value;
		}

		virtual public void Handle(IIntentState<float> obj) { }

		virtual public void Handle(IIntentState<DateTime> obj) { }

		virtual public void Handle(IIntentState<Color> obj) { }

		virtual public void Handle(IIntentState<long> obj) { }

		virtual public void Handle(IIntentState<double> obj) { }
	}
}
