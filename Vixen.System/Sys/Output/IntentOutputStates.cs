using System.Collections.Generic;

namespace Vixen.Sys.Output {
	class IntentOutputStates {
		private Dictionary<IntentOutput, IIntent[]> _outputStates;

		public IntentOutputStates() {
			_outputStates = new Dictionary<IntentOutput, IIntent[]>();
		}

		public IIntent[] GetOutputCurrentState(IntentOutput output) {
			IIntent[] outputCurrentState;
			_outputStates.TryGetValue(output, out outputCurrentState);
			return outputCurrentState;
		}

		public void SetOutputCurrentState(IntentOutput output, IIntent[] state) {
			_outputStates[output] = state;
		}
	}
}
