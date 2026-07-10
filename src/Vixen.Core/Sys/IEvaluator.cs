using Vixen.Commands;

namespace Vixen.Sys
{
	/// <summary>
	/// Evaluates an intent's state and produces a specific type of command
	/// appropriate for the controller.
	/// </summary>
	public interface IEvaluator : IDispatchable
	{
		ICommand Evaluate(IIntentState intentState);
	}
}