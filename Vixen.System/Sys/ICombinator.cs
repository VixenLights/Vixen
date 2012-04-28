using System.Collections.Generic;

namespace Vixen.Sys {
	public interface ICombinator : IDispatchable {
		void Combine(IEnumerable<IEvaluator> evaluators);
		object Value { get; }
	}

	public interface ICombinator<out T> : ICombinator {
		T Value { get; }
	}
}
