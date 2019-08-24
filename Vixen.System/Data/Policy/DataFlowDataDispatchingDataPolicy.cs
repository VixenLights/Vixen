using System.Collections.Generic;
using System.Linq;
using Vixen.Commands;
using Vixen.Data.Flow;
using Vixen.Sys;
using Vixen.Sys.Dispatch;

namespace Vixen.Data.Policy
{
	// It used to be that a controller took in a single form of data -- commands.
	// This adds the behavior of allowing three kinds of data -- single command,
	// multiple commands, multiple intents -- and dispatching it to the correct
	// point in the output pipeline (for lack of a better term):
	//
	// Single command - You may pass.
	// Multiple commands - Combine them into one command which may then pass through.
	// Multiple intents - Evaluate them to commands, then combine.

	public abstract class DataFlowDataDispatchingDataPolicy : DataFlowDataDispatch, IDataPolicy
	{
		private ICommand _commandResult;
		private IEvaluator _evaluator;
		private ICombinator _combinator;
		private readonly List<ICommand> _commands = new List<ICommand>(2);

		public ICommand GenerateCommand(IDataFlowData dataFlowData)
		{
			if (dataFlowData != null) {
				dataFlowData.Dispatch(this);
			}

			return _commandResult;
		}

		public override void Handle(CommandDataFlowData obj)
		{
			if (obj != null) {
				_commandResult = obj.Value;
			}
		}

		public override void Handle(CommandsDataFlowData obj)
		{
			if (obj != null) {
				_commandResult = CombineCommands(obj.Value);
			}
		}

		public override void Handle(IntentsDataFlowData obj)
		{
			if (obj != null) {
				List<ICommand> intentStates = EvaluateIntentStates(obj.Value);
				_commandResult = CombineCommands(intentStates);
			}
			else
			{
				_commandResult = null;
			}
		}
		public override void Handle(IntentDataFlowData obj)
		{
			if (obj != null)
			{
				//We only have one intent, so no need to combine, just evaluate and move on
				_commandResult = EvaluateIntentState(obj.Value);
			}
			else
			{
				_commandResult = null;
			}
		}

		protected internal List<ICommand> EvaluateIntentStates(List<IIntentState> intentStates)
		{
			_commands.Clear();
			foreach (var intentState in intentStates)
			{
				_commands.Add(_GetEvaluator().Evaluate(intentState));
			}

			return _commands;
		}

		protected internal ICommand EvaluateIntentState(IIntentState intentState)
		{
			return _GetEvaluator().Evaluate(intentState);
		}

		protected internal ICommand CombineCommands(List<ICommand> commands)
		{
			return _GetCombinator().Combine(commands);
		}

		// May want to use a static value or create a new value every time, hence these
		// private methods to decouple the caller.
		private IEvaluator _GetEvaluator()
		{
			return _evaluator ?? (_evaluator = GetEvaluator());
		}

		private ICombinator _GetCombinator()
		{
			return _combinator ?? (_combinator = GetCombinator());
		}

		protected abstract IEvaluator GetEvaluator();

		protected abstract ICombinator GetCombinator();
	}
}