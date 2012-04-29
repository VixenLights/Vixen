using Vixen.Sys;

namespace Vixen.Commands {
	public class UnsignedIntValue : Dispatchable<UnsignedIntValue>, ICommand<uint> {
		public UnsignedIntValue(uint value) {
			CommandValue = value;
		}

		public uint CommandValue { get; set; }

		object ICommand.CommandValue {
			get { return CommandValue; }
			set { CommandValue = (uint)value; }
		}


		//public void Dispatch(CommandDispatch commandDispatch) {
		//    // Must be done in the classes being dispatched.
		//    if(commandDispatch != null)
		//        commandDispatch.DispatchCommand(this);
		//}
	}
}
