using System.Collections.Generic;

namespace Vixen.Sys {
	public interface IIntentStates : IEnumerable<IIntentState> {
		void AddIntentState(IIntentState intentState);
	}
}
