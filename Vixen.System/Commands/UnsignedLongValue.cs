using Vixen.Sys;

namespace Vixen.Commands {
	public class UnsignedLongValue : Dispatchable<UnsignedLongValue>, ICommand<ulong> {
		public UnsignedLongValue(ulong value) {
			Value = value;
		}

		public ulong Value { get; set; }

		object ICommand.Value {
			get { return Value; }
			set { Value = (ulong)value; }
		}

		//public void Dispatch(CommandDispatch commandDispatch) {
		//    // Must be done in the classes being dispatched.
		//    if(commandDispatch != null)
		//        commandDispatch.DispatchCommand(this);
		//}
	}
}
