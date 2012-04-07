using System;
using System.Collections.Generic;
using Vixen.Sys;

namespace Vixen.Intent {
	abstract class IntentState<T, ResultType> : Dispatchable<T>, IIntentState<ResultType>
		where T : IntentState<T, ResultType> {
		protected IntentState(TimeSpan intentRelativeTime) {
			RelativeTime = intentRelativeTime;
			FilterStates = new List<IFilterState>();
			SubordinateIntentStates = new List<SubordinateIntentState>();
		}

		public TimeSpan RelativeTime { get; private set; }
		public List<IFilterState> FilterStates { get; private set; }
		public List<SubordinateIntentState> SubordinateIntentStates { get; private set; }
		abstract public ResultType GetValue();

		public IIntentState Clone() {
			IntentState<T, ResultType> newIntentState = _Clone();

			// These two steps probably aren't necessary.
			//if(VixenSystem.AllowFilterEvaluation) {
			//    newIntentState.FilterStates.AddRange(FilterStates);
			//} else {
			//    newIntentState.FilterStates.Clear();
			//}

			//if(VixenSystem.AllowSubordinateEffects) {
			//    newIntentState.SubordinateIntentStates.AddRange(SubordinateIntentStates);
			//} else {
			//    newIntentState.SubordinateIntentStates.Clear();
			//}
			newIntentState.FilterStates.AddRange(FilterStates);
			newIntentState.SubordinateIntentStates.AddRange(SubordinateIntentStates);

			return newIntentState;
		}

		abstract protected IntentState<T, ResultType> _Clone();
	}
}
