using Vixen.Sys;

namespace Vixen.Commands {
	public class UnsignedShortValue : Dispatchable<UnsignedShortValue>, ICommand<ushort> {
		public UnsignedShortValue(ushort value) {
			CommandValue = value;
		}

		public ushort CommandValue { get; set; }

		object ICommand.CommandValue {
			get { return CommandValue; }
			set { CommandValue = (ushort)value; }
		}

		//public void Dispatch(CommandDispatch commandDispatch) {
		//    // Must be done in the classes being dispatched.
		//    if(commandDispatch != null)
		//        commandDispatch.DispatchCommand(this);
		//}
	}
}
