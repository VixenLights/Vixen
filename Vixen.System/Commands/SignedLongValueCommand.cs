using Vixen.Sys;

namespace Vixen.Commands {
	public class SignedLongValueCommand : Dispatchable<SignedLongValueCommand>, ICommand<long> {
		public SignedLongValueCommand(long value) {
			CommandValue = value;
		}

		public long CommandValue { get; set; }

		object ICommand.CommandValue {
			get { return CommandValue; }
			set { CommandValue = (long)value; }
		}

		//public void Dispatch(CommandDispatch commandDispatch) {
		//    // Must be done in the classes being dispatched.
		//    if(commandDispatch != null)
		//        commandDispatch.DispatchCommand(this);
		//}
	}
}
