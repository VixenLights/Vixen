using System.Collections.Generic;
using Vixen.Commands;
using Vixen.Sys;
using Vixen.Sys.Dispatch;

namespace Vixen.Data.Combinator {
	abstract public class Combinator<T, ResultType> : Dispatchable<T>, ICombinator<ResultType>, IAnyEvaluatorHandler
		where T : Combinator<T, ResultType>  {
		public void Combine(IEnumerable<ICommand> evaluators) {
			CombinatorValue = null;

			foreach(IEvaluator evaluator in evaluators) {
				evaluator.Dispatch(this);
			}
		}

		//Can't be a float, must be discrete, digital as it's the type going to the controller.
		//The types need to match the types wrapped by the commands.
		//virtual public void Handle(IEvaluator<float> obj) { }

		virtual public void Handle(IEvaluator<byte> obj) { }

		virtual public void Handle(IEvaluator<ushort> obj) { }

		virtual public void Handle(IEvaluator<uint> obj) { }

		virtual public void Handle(IEvaluator<ulong> obj) { }

		virtual public void Handle(IEvaluator<System.Drawing.Color> obj) { }

		public ICommand<ResultType> CombinatorValue { get; protected set; }

		ICommand ICombinator.CombinatorValue {
			get { return CombinatorValue; }
		}
	}
}
