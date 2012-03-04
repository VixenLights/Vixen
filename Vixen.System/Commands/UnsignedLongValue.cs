using Vixen.Sys;

namespace Vixen.Commands {
	public class UnsignedLongValue : Dispatchable<UnsignedLongValue>, ICommand {
		public UnsignedLongValue(ulong value) {
			Value = value;
		}

		public ulong Value { get; private set; }

		//public void Dispatch(CommandDispatch commandDispatch) {
		//    // Must be done in the classes being dispatched.
		//    if(commandDispatch != null)
		//        commandDispatch.DispatchCommand(this);
		//}
	}
}
