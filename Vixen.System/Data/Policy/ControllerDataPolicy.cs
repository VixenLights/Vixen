using System.Collections.Generic;
using Vixen.Commands;
using Vixen.Sys;

namespace Vixen.Data.Policy {
	public abstract class ControllerDataPolicy : IDataPolicy {
		virtual public ICommand GenerateCommand(IEnumerable<IIntentState> intentStates) {
			// Stage 1: Evaluate into a single type of data (possibly transforming).
			IEnumerable<IEvaluator> evaluators = EvaluateIntentStates(intentStates);
			// Stage 2: Combine values of that type.
			ICombinator combinator = CombineEvaluations(evaluators);

			return combinator.CombinatorValue;
		}

		protected internal virtual IEnumerable<IEvaluator> EvaluateIntentStates(IEnumerable<IIntentState> intentStates) {
			foreach(IIntentState intentState in intentStates) {
				IEvaluator evaluator = _GetEvaluator();
				evaluator.Evaluate(intentState);
				yield return evaluator;
			}
		}

		protected internal virtual ICombinator CombineEvaluations(IEnumerable<IEvaluator> evaluators) {
			ICombinator combinator = _GetCombinator();
			combinator.Combine(evaluators);
			return combinator;
		}

		// May want to use a static value or create a new value every time, hence these
		// private methods to decouple the caller.
		private IEvaluator _GetEvaluator() {
			return GetEvaluator();
		}

		private ICombinator _GetCombinator() {
			return GetCombinator();
		}

		abstract protected IEvaluator GetEvaluator();

		abstract protected ICombinator GetCombinator();
	}
}
