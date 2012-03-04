using Vixen.Sys;

namespace Vixen.Commands {
	public class UnsignedShortValue : Dispatchable<UnsignedShortValue>, ICommand {
		public UnsignedShortValue(ushort value) {
			Value = value;
		}

		public ushort Value { get; private set; }

		//public void Dispatch(CommandDispatch commandDispatch) {
		//    // Must be done in the classes being dispatched.
		//    if(commandDispatch != null)
		//        commandDispatch.DispatchCommand(this);
		//}
	}
}
