using Vixen.Sys;

namespace Vixen.Commands {
	public class SignedLongValue : Dispatchable<SignedLongValue>, ICommand<long> {
		public SignedLongValue(long value) {
			Value = value;
		}

		public long Value { get; set; }

		object ICommand.Value {
			get { return Value; }
			set { Value = (long)value; }
		}

		//public void Dispatch(CommandDispatch commandDispatch) {
		//    // Must be done in the classes being dispatched.
		//    if(commandDispatch != null)
		//        commandDispatch.DispatchCommand(this);
		//}
	}
}
