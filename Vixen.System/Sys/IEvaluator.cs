namespace Vixen.Sys {
	public interface IEvaluator : IDispatchable {
		void Evaluate(IIntentState intentState);
		object Value { get; }
	}

	public interface IEvaluator<out T> : IEvaluator {
		T Value { get; }
	}
}
