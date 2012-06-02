using System;
using Vixen.Commands;
using Vixen.Intents.Interpolators;

namespace Vixen.Intents {
	public class SignedShortTransitionIntent : TransitionIntent<short> {
		public SignedShortTransitionIntent(short startValue, short endValue, TimeSpan timeSpan)
			: base(startValue, endValue, timeSpan, new SignedShortInterpolator()) {
		}

		protected override ICommand GetCommandForValue(short value) {
			return new SignedShortValue(value);
		}
		//override public void Dispatch(IntentDispatch intentDispatch) {
		//    // Must be done in the classes being dispatched.
		//    if(intentDispatch != null)
		//        intentDispatch.DispatchIntent(this);
		//}
	}
}
