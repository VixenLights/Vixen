using Vixen.Commands;
using Vixen.Data.Value;
using Vixen.Sys;
using Vixen.Sys.Dispatch;

namespace Vixen.Data.Evaluator {
	public abstract class Evaluator<T, ResultType> : Dispatchable<T>, IEvaluator<ResultType>, IAnyIntentStateHandler
		where T : Evaluator<T, ResultType> {
		public void Evaluate(IIntentState intentState) {
			intentState.Dispatch(this);
		}

		// Opt-in, not opt-out.  Default handlers will not be called
		// from the base class.

		virtual public void Handle(IIntentState<ColorValue> obj) { }

		virtual public void Handle(IIntentState<LightingValue> obj) { }

		virtual public void Handle(IIntentState<PositionValue> obj) { }

		virtual public void Handle(IIntentState<CommandValue> obj) { }

		public ICommand<ResultType> EvaluatorValue { get; protected set; }

		object IEvaluator.EvaluatorValue {
			get { return EvaluatorValue; }
		}
	}
}
