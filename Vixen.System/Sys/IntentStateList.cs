using System.Collections.Generic;

namespace Vixen.Sys {
	public class IntentStateList : List<IIntentState>, IIntentStates {
		public IntentStateList() {
		}

		public IntentStateList(IEnumerable<IIntentState> states) {
			foreach(IIntentState intentState in states) {
				AddIntentState(intentState);
			}
		}

		virtual public void AddIntentState(IIntentState intentState) {
			Add(intentState);
		}
	}
}
