using Vixen.Sys;

namespace Vixen.Commands {
	public class SignedIntValue : Dispatchable<SignedIntValue>, ICommand<int> {
		public SignedIntValue(int value) {
			Value = value;
		}

		public int Value { get; set; }

		object ICommand.Value {
			get { return Value; }
			set { Value = (int)value; }
		}

		//public void Dispatch(CommandDispatch commandDispatch) {
		//    // Must be done in the classes being dispatched.
		//    if(commandDispatch != null)
		//        commandDispatch.DispatchCommand(this);
		//}
	}
}
