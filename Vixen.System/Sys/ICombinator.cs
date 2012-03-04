using System.Collections.Generic;

namespace Vixen.Sys {
	public interface ICombinator : IDispatchable {
		void Combine(IEnumerable<IEvaluator> evaluators);
	}

	interface ICombinator<out T> : ICombinator {
		T Value { get; }
	}
}
