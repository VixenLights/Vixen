using System;
using Vixen.Commands;
using Vixen.Intents.Interpolators;

namespace Vixen.Intents {
	public class UnsignedShortTransitionIntent : TransitionIntent<ushort> {
		public UnsignedShortTransitionIntent(ushort startValue, ushort endValue, TimeSpan timeSpan)
			: base(startValue, endValue, timeSpan, new UnsignedShortInterpolator()) {
		}

		protected override ICommand GetCommandForValue(ushort value) {
			return new UnsignedShortValue(value);
		}
		//override public void Dispatch(IntentDispatch intentDispatch) {
		//    // Must be done in the classes being dispatched.
		//    if(intentDispatch != null)
		//        intentDispatch.DispatchIntent(this);
		//}
	}
}
