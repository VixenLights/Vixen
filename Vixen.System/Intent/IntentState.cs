using System;
using Vixen.Data.Value;
using Vixen.Sys;

namespace Vixen.Intent {
	class IntentState<ResultType> : Dispatchable<IntentState<ResultType>>, IIntentState<ResultType>
		where ResultType : IIntentDataType {
		public IntentState(IIntent<ResultType> intent, TimeSpan intentRelativeTime) {
			if(intent == null) throw new ArgumentNullException("intent");

			Intent = intent;
			RelativeTime = intentRelativeTime;
		}

		public IIntent<ResultType> Intent { get; private set; }

		IIntent IIntentState.Intent {
			get { return Intent; }
		}

		public TimeSpan RelativeTime { get; private set; }
		
		public ResultType GetValue() {
			return Intent.GetStateAt(RelativeTime);
		}

		object IIntentState.GetValue() {
			return GetValue();
		}

		public IIntentState Clone() {
			IntentState<ResultType> newIntentState = new IntentState<ResultType>(Intent, RelativeTime);

			return newIntentState;
		}
	}
}
