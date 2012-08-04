namespace Vixen.Sys.Output {
	public class IntentOutput : StateHoldingIntentOutput {
		public IntentChangeCollection IntentChangeCollection { get; set; }

		public void LogicalFiltering() {
			// No filters on intent outputs currently.
		}
	}
}
