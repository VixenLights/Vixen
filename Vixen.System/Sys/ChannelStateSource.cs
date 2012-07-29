namespace Vixen.Sys {
	public class ChannelStateSource : IChannelStateSource {
		//public ChannelStateSource() {
		//}

		//public ChannelStateSource(IntentStateList state) {
		//    State = state;
		//}

		public IIntentStates State { get; set; }
	}
}
