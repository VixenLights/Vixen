using System.Collections.Generic;

namespace Vixen.Sys {
	public class IntentStateList : List<IIntentState>, IIntentStateList {
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

		/// <summary>
		/// Adds filter states to every intent state.
		/// </summary>
		/// <param name="filterStates"></param>
		public void AddFilters(IEnumerable<IFilterState> filterStates) {
			if(VixenSystem.AllowFilterEvaluation) {
				foreach(IIntentState intentState in this) {
					intentState.FilterStates.AddRange(filterStates);
				}
			}
		}
	}
}
