using System;
using System.Collections.Generic;
using Vixen.Sys;

namespace Vixen.Intent {
	//class IntentState<IntentStateType, ResultType> : Dispatchable<IntentStateType>, IIntentState<ResultType>
	//    where IntentStateType : IntentState<IntentStateType, ResultType> {
	class IntentState<ResultType> : Dispatchable<IntentState<ResultType>>, IIntentState<ResultType> {
		public IntentState(IIntent<ResultType> intent, TimeSpan intentRelativeTime) {
			if(intent == null) throw new ArgumentNullException("intent");

			Intent = intent;
			RelativeTime = intentRelativeTime;
			SubordinateIntentStates = new List<SubordinateIntentState>();
		}

		public IIntent<ResultType> Intent { get; private set; }

		IIntent IIntentState.Intent {
			get { return Intent; }
		}

		public TimeSpan RelativeTime { get; private set; }
		
		public List<SubordinateIntentState> SubordinateIntentStates { get; private set; }
	
		//abstract public ResultType GetValue();
		public ResultType GetValue() {
			return Intent.GetStateAt(RelativeTime);
		}

		object IIntentState.GetValue() {
			return GetValue();
		}

		public IIntentState Clone() {
			//IntentState<IntentStateType, ResultType> newIntentState = _Clone();
			//IntentState<IntentStateType, ResultType> newIntentState = new IntentState<IntentStateType, ResultType>(Intent, RelativeTime);
			IntentState<ResultType> newIntentState = new IntentState<ResultType>(Intent, RelativeTime);

			newIntentState.SubordinateIntentStates.AddRange(SubordinateIntentStates);

			return newIntentState;
		}

		//abstract protected IntentState<IntentStateType, ResultType> _Clone();
	}
}
