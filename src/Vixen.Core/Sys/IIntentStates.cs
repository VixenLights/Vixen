namespace Vixen.Sys
{
	public interface IIntentStates : IList<IIntentState>
	{
		void AddIntentState(IIntentState intentState);
		new IIntentState this[int index] { get; set; }
		List<IIntentState> AsList();
	}
}