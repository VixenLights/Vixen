using Vixen.Intents;

namespace Vixen.Sys {
	abstract public class IntentDispatch : IHandler<ByteTransitionIntent>, IHandler<SignedShortTransitionIntent>, IHandler<UnsignedShortTransitionIntent>, IHandler<SignedIntTransitionIntent>, IHandler<UnsignedIntTransitionIntent>, IHandler<SignedLongTransitionIntent>, IHandler<UnsignedLongTransitionIntent> {
		//public virtual void DispatchIntent<T>(IIntent command) {
		//}
		virtual public void Handle(ByteTransitionIntent c) {
		}

		virtual public void DispatchCommand(SignedShortTransitionIntent c) {
		}

		virtual public void DispatchCommand(UnsignedShortTransitionIntent c) {
		}

		virtual public void DispatchCommand(SignedIntTransitionIntent c) {
		}

		virtual public void DispatchCommand(UnsignedIntTransitionIntent c) {
		}

		virtual public void DispatchCommand(SignedLongTransitionIntent c) {
		}

		virtual public void DispatchCommand(UnsignedLongTransitionIntent c) {
		}
	}
}
