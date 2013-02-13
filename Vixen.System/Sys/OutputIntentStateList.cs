using System.Collections.Generic;

namespace Vixen.Sys {
	class OutputIntentStateList : IntentStateList {
		public OutputIntentStateList(IEnumerable<IIntentState> intentStates)
			: base(intentStates) {
		}

		override public void AddIntentState(IIntentState intentState) {
			// Clone the intent state so that outputs using the same elements don't
			// use the same intent and clobber each other.
			Add(intentState.Clone());
		}
	}
}
