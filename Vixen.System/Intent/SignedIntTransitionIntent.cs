using System;
using Vixen.Commands;
using Vixen.Intents.Interpolators;

namespace Vixen.Intents {
	public class SignedIntTransitionIntent : TransitionIntent<int> {
		public SignedIntTransitionIntent(int startValue, int endValue, TimeSpan timeSpan)
			: base(startValue, endValue, timeSpan, new SignedIntInterpolator()) {
		}

		protected override ICommand GetCommandForValue(int value) {
			return new SignedIntValue(value);
		}
		//override public void Dispatch(IntentDispatch intentDispatch) {
		//    // Must be done in the classes being dispatched.
		//    if(intentDispatch != null)
		//        intentDispatch.DispatchIntent(this);
		//}
	}
}
