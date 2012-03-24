using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Vixen.Sys.CombinationOperation;

namespace Vixen.Sys {
	public class SubordinateIntentState {
		public SubordinateIntentState(IIntentState intentState, ICombinationOperation combinationOperation) {
			IntentState = intentState;
			CombinationOperation = combinationOperation;
		}

		public IIntentState IntentState { get; private set; }

		public ICombinationOperation CombinationOperation { get; private set; }

		static public long Aggregate(long initialValue, IEnumerable<SubordinateIntentState> subordinateIntentStates) {
			return subordinateIntentStates.Aggregate(initialValue, (current, subordinateIntentState) => subordinateIntentState.CombinationOperation.Combine(current, subordinateIntentState.IntentState));
		}

		static public float Aggregate(float initialValue, IEnumerable<SubordinateIntentState> subordinateIntentStates) {
			return subordinateIntentStates.Aggregate(initialValue, (current, subordinateIntentState) => subordinateIntentState.CombinationOperation.Combine(current, subordinateIntentState.IntentState));
		}

		static public double Aggregate(double initialValue, IEnumerable<SubordinateIntentState> subordinateIntentStates) {
			return subordinateIntentStates.Aggregate(initialValue, (current, subordinateIntentState) => subordinateIntentState.CombinationOperation.Combine(current, subordinateIntentState.IntentState));
		}

		static public DateTime Aggregate(DateTime initialValue, IEnumerable<SubordinateIntentState> subordinateIntentStates) {
			return subordinateIntentStates.Aggregate(initialValue, (current, subordinateIntentState) => subordinateIntentState.CombinationOperation.Combine(current, subordinateIntentState.IntentState));
		}

		static public Color Aggregate(Color initialValue, IEnumerable<SubordinateIntentState> subordinateIntentStates) {
			return subordinateIntentStates.Aggregate(initialValue, (current, subordinateIntentState) => subordinateIntentState.CombinationOperation.Combine(current, subordinateIntentState.IntentState));
		}
	}
}
