using Vixen.Sys;

namespace Vixen.Commands {
	public class SignedLongValue : Dispatchable<SignedLongValue>, ICommand {
		public SignedLongValue(long value) {
			Value = value;
		}

		public long Value { get; private set; }

		//public void Dispatch(CommandDispatch commandDispatch) {
		//    // Must be done in the classes being dispatched.
		//    if(commandDispatch != null)
		//        commandDispatch.DispatchCommand(this);
		//}
	}
}
