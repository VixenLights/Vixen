namespace Vixen.Sys.Output {
	public class StateHoldingIntentOutput : Output {
		public IIntent[] LastSetState { get; set; }
	}
}
