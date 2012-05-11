using Vixen.Sys;

namespace Vixen.Commands {
	public class SignedIntValueCommand : Dispatchable<SignedIntValueCommand>, ICommand<int> {
		public SignedIntValueCommand(int value) {
			CommandValue = value;
		}

		public int CommandValue { get; set; }

		object ICommand.CommandValue {
			get { return CommandValue; }
			set { CommandValue = (int)value; }
		}

		//public void Dispatch(CommandDispatch commandDispatch) {
		//    // Must be done in the classes being dispatched.
		//    if(commandDispatch != null)
		//        commandDispatch.DispatchCommand(this);
		//}
	}
}
