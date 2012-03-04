namespace Vixen.Sys {
	public interface IEvaluator : IDispatchable {
		void Evaluate(IIntentState intentState);
	}

	interface IEvaluator<out T> : IEvaluator {
		T Value { get; }
	}
}
