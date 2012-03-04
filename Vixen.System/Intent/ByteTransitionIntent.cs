using System;
using Vixen.Commands;
using Vixen.Intents.Interpolators;

namespace Vixen.Intents {
	public class ByteTransitionIntent : TransitionIntent<byte> {
		public ByteTransitionIntent(byte startValue, byte endValue, TimeSpan timeSpan)
			: base(startValue, endValue, timeSpan, new ByteInterpolator()) {
		}

		protected override ICommand GetCommandForValue(byte value) {
			return new ByteValue(value);
		}

		//override public void Dispatch(IntentDispatch intentDispatch) {
		//    // Must be done in the classes being dispatched.
		//    if(intentDispatch != null)
		//        intentDispatch.DispatchIntent(this);
		//}
	}
}
