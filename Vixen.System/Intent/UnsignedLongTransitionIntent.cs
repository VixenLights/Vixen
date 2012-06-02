using System;
using Vixen.Commands;
using Vixen.Intents.Interpolators;
using Vixen.Sys;

namespace Vixen.Intents {
	public class UnsignedLongTransitionIntent : TransitionIntent<ulong> {
		public UnsignedLongTransitionIntent(ulong startValue, ulong endValue, TimeSpan timeSpan)
			: base(startValue, endValue, timeSpan, new UnsignedLongInterpolator()) {
		}

		protected override ICommand GetCommandForValue(ulong value) {
			return new UnsignedLongValue(value);
		}
		//override public void Dispatch(IntentDispatch intentDispatch) {
		//    // Must be done in the classes being dispatched.
		//    if(intentDispatch != null)
		//        intentDispatch.DispatchIntent(this);
		//}
	}
}
