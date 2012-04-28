using Vixen.Sys;

namespace Vixen.Commands {
	public class UnsignedIntValue : Dispatchable<UnsignedIntValue>, ICommand<uint> {
		public UnsignedIntValue(uint value) {
			Value = value;
		}

		public uint Value { get; set; }

		object ICommand.Value {
			get { return Value; }
			set { Value = (uint)value; }
		}


		//public void Dispatch(CommandDispatch commandDispatch) {
		//    // Must be done in the classes being dispatched.
		//    if(commandDispatch != null)
		//        commandDispatch.DispatchCommand(this);
		//}
	}
}
