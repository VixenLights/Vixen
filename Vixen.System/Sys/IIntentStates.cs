using System.Collections.Generic;

namespace Vixen.Sys
{
	public interface IIntentStates : IList<IIntentState>
	{
		void AddIntentState(IIntentState intentState);
		IIntentState this[int index] { get; set; }
		List<IIntentState> AsList();
	}
}