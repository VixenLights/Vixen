using Vixen.Sys;

namespace Vixen.Commands {
	public class SignedIntValue : Dispatchable<SignedIntValue>, ICommand {
		public SignedIntValue(int value) {
			Value = value;
		}

		public int Value { get; private set; }

		//public void Dispatch(CommandDispatch commandDispatch) {
		//    // Must be done in the classes being dispatched.
		//    if(commandDispatch != null)
		//        commandDispatch.DispatchCommand(this);
		//}
	}
}
