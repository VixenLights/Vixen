using Vixen.Commands;

namespace Vixen.Sys {
	/// <summary>
	/// Evaluates an intent's state and produces a specific type of command
	/// appropriate for the controller.
	/// </summary>
	public interface IEvaluator : IDispatchable {
		void Evaluate(IIntentState intentState);
		ICommand EvaluatorValue { get; }
	}

	/// <summary>
	/// Evaluates an intent's state and produces a specific type of command
	/// appropriate for the controller.
	/// </summary>
	public interface IEvaluator<T> : IEvaluator {
		ICommand<T> EvaluatorValue { get; }
	}
}
