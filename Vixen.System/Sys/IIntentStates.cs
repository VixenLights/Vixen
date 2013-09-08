using System.Collections.Generic;

namespace Vixen.Sys
{
	public interface IIntentStates : IEnumerable<IIntentState>
	{
		void AddIntentState(IIntentState intentState);
		void AddRangeIntentState(IEnumerable<IIntentState> intentStates);
		void ClearStates();
		IIntentState this[int index] { get; set; }
		int Count { get; }
	}
}