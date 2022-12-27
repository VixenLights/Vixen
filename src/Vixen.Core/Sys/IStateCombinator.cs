namespace Vixen.Sys
{
	public interface IStateCombinator: IDispatchable
	{
		List<IIntentState> Combine(List<IIntentState> states);
	}
}
