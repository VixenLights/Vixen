using System.Collections.Generic;
using Vixen.Commands;

namespace Vixen.Sys {
	public interface ICombinator : IDispatchable {
		void Combine(IEnumerable<IEvaluator> evaluators);
		ICommand CombinatorValue { get; }
	}

	public interface ICombinator<T> : ICombinator {
		ICommand<T> CombinatorValue { get; }
	}
}
