using Vixen.Sys;

namespace Vixen.Commands {
	public class UnsignedIntValue : Dispatchable<UnsignedIntValue>, ICommand {
		public UnsignedIntValue(uint value) {
			Value = value;
		}

		public uint Value { get; private set; }

		//public void Dispatch(CommandDispatch commandDispatch) {
		//    // Must be done in the classes being dispatched.
		//    if(commandDispatch != null)
		//        commandDispatch.DispatchCommand(this);
		//}
	}
}
