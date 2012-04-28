using System;
using System.Collections.Generic;
using Vixen.Sys;

namespace Vixen.Intent {
	abstract class IntentState<IntentStateType, ResultType> : Dispatchable<IntentStateType>, IIntentState<ResultType>
		where IntentStateType : IntentState<IntentStateType, ResultType> {
		protected IntentState(TimeSpan intentRelativeTime) {
			RelativeTime = intentRelativeTime;
			SubordinateIntentStates = new List<SubordinateIntentState>();
		}

		public TimeSpan RelativeTime { get; private set; }
		
		public List<SubordinateIntentState> SubordinateIntentStates { get; private set; }
	
		abstract public ResultType GetValue();

		object IIntentState.GetValue() {
			return GetValue();
		}

		public IIntentState Clone() {
			IntentState<IntentStateType, ResultType> newIntentState = _Clone();

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
			newIntentState.SubordinateIntentStates.AddRange(SubordinateIntentStates);

			return newIntentState;
		}

		abstract protected IntentState<IntentStateType, ResultType> _Clone();
	}
}
