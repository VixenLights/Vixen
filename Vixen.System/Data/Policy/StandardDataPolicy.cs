using System.Collections.Generic;
using Vixen.Commands;
using Vixen.Sys;

namespace Vixen.Data.Policy {
	abstract public class StandardDataPolicy : IDataPolicy {
		public ICommand GenerateCommand(IEnumerable<IIntentState> intentStates) {
			IEnumerable<IEvaluator> evaluators = _EvaluateIntentStates(intentStates);
			//Either need to create a new combinator and generator every time or lock
			//around the combinator and generator because they have state that would
			//be shared by multiple threads.
			ICombinator combinator = _CombineEvaluations(evaluators);
			ICommand command = _GenerateCommandFromCombinator(combinator);

			return command;
		}

		protected virtual IEnumerable<IEvaluator> _EvaluateIntentStates(IEnumerable<IIntentState> intentStates) {
			foreach(IIntentState intentState in intentStates) {
				IEvaluator evaluator = _GetEvaluator();
				evaluator.Evaluate(intentState);
				yield return evaluator;
			}
		}

		protected virtual ICombinator _CombineEvaluations(IEnumerable<IEvaluator> evaluators) {
			ICombinator combinator = _GetCombinator();
			combinator.Combine(evaluators);
			return combinator;
		}

		protected virtual ICommand _GenerateCommandFromCombinator(ICombinator combinator) {
			IGenerator generator = _GetGenerator();
			generator.GenerateCommand(combinator);
			return generator.Value;
		}

		private IEvaluator _GetEvaluator() {
			return GetEvaluator();
		}

		private ICombinator _GetCombinator() {
			//return _combinator ?? (_combinator = GetCombinator());
			return GetCombinator();
		}

		private IGenerator _GetGenerator() {
			//return _generator ?? (_generator = GetGenerator());
			return GetGenerator();
		}

		abstract protected IEvaluator GetEvaluator();

		abstract protected ICombinator GetCombinator();

		abstract protected IGenerator GetGenerator();
	}
}
