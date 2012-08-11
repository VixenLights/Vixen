using System.Collections.Generic;
using Vixen.Commands;
using Vixen.Data.Flow;
using Vixen.Sys;
using Vixen.Sys.Dispatch;

namespace Vixen.Data.Policy {
	abstract public class OutputStateDispatchingDataPolicy : DataFlowDataDispatch, IDataPolicy {
		private ICommand _commandResult;

		//virtual public ICommand GenerateCommand(IEnumerable<IIntentState> intentStates) {
		//    // Stage 1: Evaluate into a single type of data (possibly transforming).
		//    IEnumerable<IEvaluator> evaluators = EvaluateIntentStates(intentStates);
		//    // Stage 2: Combine values of that type.
		//    ICombinator combinator = CombineEvaluations(evaluators);

		//    return combinator.CombinatorValue;
		//}

		public ICommand GenerateCommand(IDataFlowData dataFlowData) {
			if(dataFlowData != null) {
				dataFlowData.Dispatch(this);
			}
			return _commandResult;
		}

		public override void Handle(IDataFlowData<ICommand> obj) {
			if(obj != null) {
				_commandResult = obj.Value;
			}
		}

		public override void Handle(IDataFlowData<IEnumerable<ICommand>> obj) {
			if(obj != null) {
				_commandResult = CombineCommands(obj.Value);
			}
		}

		public override void Handle(IDataFlowData<IEnumerable<IIntentState>> obj) {
			if(obj != null) {
				IEnumerable<ICommand> intentStates = EvaluateIntentStates(obj.Value);
				_commandResult = CombineCommands(intentStates);
			}
		}

		//protected internal virtual IEnumerable<IEvaluator> EvaluateIntentStates(IEnumerable<IIntentState> intentStates) {
		//    foreach(IIntentState intentState in intentStates) {
		//        IEvaluator evaluator = _GetEvaluator();
		//        evaluator.Evaluate(intentState);
		//        yield return evaluator;
		//    }
		//}
		protected internal virtual IEnumerable<ICommand> EvaluateIntentStates(IEnumerable<IIntentState> intentStates) {
			foreach(IIntentState intentState in intentStates) {
				IEvaluator evaluator = _GetEvaluator();
				evaluator.Evaluate(intentState);
				yield return evaluator.EvaluatorValue;
			}
		}

		//protected internal virtual ICombinator CombineEvaluations(IEnumerable<IEvaluator> evaluators) {
		//    ICombinator combinator = _GetCombinator();
		//    combinator.Combine(evaluators);
		//    return combinator;
		//}
		protected internal virtual ICommand CombineCommands(IEnumerable<ICommand> commands) {
			ICombinator combinator = _GetCombinator();
			combinator.Combine(commands);
			return combinator.CombinatorValue;
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
