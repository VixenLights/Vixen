using System.Collections.Generic;

namespace Vixen.Sys {
	public interface IIntentStateList : IEnumerable<IIntentState> {
		void AddIntentState(IIntentState intentState);
		void AddFilters(IEnumerable<IFilterState> filterStates);
	}
}
