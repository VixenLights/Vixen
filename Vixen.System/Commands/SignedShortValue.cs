using Vixen.Sys;

namespace Vixen.Commands {
	public class SignedShortValue : Dispatchable<SignedShortValue>, ICommand {
		public SignedShortValue(short value) {
			Value = value;
		}

		public short Value { get; private set; }

		//public void Dispatch(CommandDispatch commandDispatch) {
		//    // Must be done in the classes being dispatched.
		//    if(commandDispatch != null)
		//        commandDispatch.DispatchCommand(this);
		//}
	}
}
