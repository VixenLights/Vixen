using System.Collections.Generic;
using System.Linq;
using Vixen.Commands;
using Vixen.Data.Flow;
using Vixen.Sys;
using Vixen.Sys.Dispatch;

namespace Vixen.Data.Policy {
	
	// It used to be that a controller took in a single form of data -- commands.
	// This adds the behavior of allowing three kinds of data -- single command,
	// multiple commands, multiple intents -- and dispatching it to the correct
	// point in the output pipeline (for lack of a better term):
	//
	// Single command - You may pass.
	// Multiple commands - Combine them into one command which may then pass through.
	// Multiple intents - Evaluate them to commands, then combine.

	abstract public class DataFlowDataDispatchingDataPolicy : DataFlowDataDispatch, IDataPolicy {
		private ICommand _commandResult;
		private IEvaluator _evaluator;
		private ICombinator _combinator;

		public ICommand GenerateCommand(IDataFlowData dataFlowData) {
			if(dataFlowData != null) {
				dataFlowData.Dispatch(this);
			}
			return _commandResult;
		}

		public override void Handle(CommandDataFlowData obj) {
			if(obj != null) {
				_commandResult = obj.Value;
			}
		}

		public override void Handle(CommandsDataFlowData obj) {
			if(obj != null) {
				_commandResult = CombineCommands(obj.Value);
			}
		}

		public override void Handle(IntentsDataFlowData obj) {
			if(obj != null) {
				IEnumerable<ICommand> intentStates = EvaluateIntentStates(obj.Value);
				_commandResult = CombineCommands(intentStates);
			}
		}

		protected internal virtual IEnumerable<ICommand> EvaluateIntentStates(IEnumerable<IIntentState> intentStates) {
			return intentStates.Select(_GetEvaluator().Evaluate);
		}

		protected internal virtual ICommand CombineCommands(IEnumerable<ICommand> commands) {
			return _GetCombinator().Combine(commands);
		}

		// May want to use a static value or create a new value every time, hence these
		// private methods to decouple the caller.
		private IEvaluator _GetEvaluator() {
			return _evaluator ?? (_evaluator = GetEvaluator());
		}

		private ICombinator _GetCombinator() {
			return _combinator ?? (_combinator = GetCombinator());
		}

		abstract protected IEvaluator GetEvaluator();

		abstract protected ICombinator GetCombinator();
	}
}
