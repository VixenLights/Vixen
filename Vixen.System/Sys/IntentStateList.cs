using System.Collections.Generic;

namespace Vixen.Sys
{
	public class IntentStateList : List<IIntentState>, IIntentStates
	{
		public IntentStateList()
		{
			
		}

		public IntentStateList(int capacity) : base(capacity)
		{

		}

		public IntentStateList(IEnumerable<IIntentState> states)
		{
			//foreach(IIntentState intentState in states) {
			//    AddIntentState(intentState);
			//}
			AddRangeIntentState(states); //Faster than for add
		}

		public virtual void AddIntentState(IIntentState intentState)
		{
			Add(intentState);
		}

		public virtual void AddRangeIntentState(IEnumerable<IIntentState> intentStates)
		{
			AddRange(intentStates);
		}

		public virtual List<IIntentState> AsList()
		{
			return this;
		} 
	}
}