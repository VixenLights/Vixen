using Vixen.Sys;

namespace Vixen.Commands {
	public class SignedShortValue : Dispatchable<SignedShortValue>, ICommand<short> {
		public SignedShortValue(short value) {
			Value = value;
		}

		public short Value { get; set; }

		object ICommand.Value {
			get { return Value; }
			set { Value = (short)value; }
		}

		//public void Dispatch(CommandDispatch commandDispatch) {
		//    // Must be done in the classes being dispatched.
		//    if(commandDispatch != null)
		//        commandDispatch.DispatchCommand(this);
		//}
	}
}
