using System;
using Vixen.Commands;
using Vixen.Intents.Interpolators;

namespace Vixen.Intents {
	public class SignedLongTransitionIntent : TransitionIntent<long> {
		public SignedLongTransitionIntent(long startValue, long endValue, TimeSpan timeSpan)
			: base(startValue, endValue, timeSpan, new SignedLongInterpolator()) {
		}

		protected override ICommand GetCommandForValue(long value) {
			return new SignedLongValue(value);
		}
		//override public void Dispatch(IntentDispatch intentDispatch) {
		//    // Must be done in the classes being dispatched.
		//    if(intentDispatch != null)
		//        intentDispatch.DispatchIntent(this);
		//}
	}
}
