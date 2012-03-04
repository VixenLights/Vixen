using System.Collections.Generic;

namespace Vixen.Sys {
	// Some explanation:
	// If outputs share intent states due to a n:1 patching, then they can't add their filters
	// to the states because they will affect the states for subsequent outputs.
	// So what this is doing is getting a list of states that will affect the output (from all
	// channels feeding into it) and a list of filters for that output.  Then as the states
	// are pulled out of here, the output's filters are added to the intents.
	class OutputIntentStateList : IIntentStateList {
		private List<IIntentState> _outputStates;
		private List<IFilterState> _filterStates;

		public OutputIntentStateList(IEnumerable<IIntentState> intentStates) {
			_outputStates = new List<IIntentState>();
			_filterStates = new List<IFilterState>();

			foreach(IIntentState intentState in intentStates) {
				AddIntentState(intentState);
			}
		}

		public void AddIntentState(IIntentState intentState) {
			// Clone the intent state.
			IIntentState newIntentState = intentState.Clone();
			// Add.
			_outputStates.Add(newIntentState);
		}
		
		public void AddFilters(IEnumerable<IFilterState> filterStates) {
			_filterStates.AddRange(filterStates);
		}

		public IEnumerator<IIntentState> GetEnumerator() {
			foreach(IIntentState intentState in _outputStates) {
				intentState.FilterStates.AddRange(_filterStates);
				yield return intentState;
			}
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
	}
}
