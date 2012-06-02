namespace Vixen.Sys {
	public class OutputStateSource : IOutputStateSource {
		public OutputStateSource(IIntentStateList state) {
			State = state;
		}

		public IIntentStateList State { get; set; }
	}
}
