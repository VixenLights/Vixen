namespace Vixen.Sys {
	public interface IEvaluator : IDispatchable {
		void Evaluate(IIntentState intentState);
		object EvaluatorValue { get; }
	}

	public interface IEvaluator<out T> : IEvaluator {
		T EvaluatorValue { get; }
	}
}
