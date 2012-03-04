using System;
using Vixen.Commands;
using Vixen.Intents.Interpolators;

namespace Vixen.Intents {
	public class UnsignedIntTransitionIntent : TransitionIntent<uint> {
		public UnsignedIntTransitionIntent(uint startValue, uint endValue, TimeSpan timeSpan)
			: base(startValue, endValue, timeSpan, new UnsignedIntInterpolator()) {
		}

		protected override ICommand GetCommandForValue(uint value) {
			return new UnsignedIntValue(value);
		}
		//override public void Dispatch(IntentDispatch intentDispatch) {
		//    // Must be done in the classes being dispatched.
		//    if(intentDispatch != null)
		//        intentDispatch.DispatchIntent(this);
		//}
	}
}
