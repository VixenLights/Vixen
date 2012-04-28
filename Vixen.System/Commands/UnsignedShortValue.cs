using Vixen.Sys;

namespace Vixen.Commands {
	public class UnsignedShortValue : Dispatchable<UnsignedShortValue>, ICommand<ushort> {
		public UnsignedShortValue(ushort value) {
			Value = value;
		}

		public ushort Value { get; set; }

		object ICommand.Value {
			get { return Value; }
			set { Value = (ushort)value; }
		}

		//public void Dispatch(CommandDispatch commandDispatch) {
		//    // Must be done in the classes being dispatched.
		//    if(commandDispatch != null)
		//        commandDispatch.DispatchCommand(this);
		//}
	}
}
