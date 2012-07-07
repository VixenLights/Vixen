using System;
using System.Collections.Generic;
using System.Drawing;
using Vixen.Commands;
using Vixen.Sys;
using Vixen.Sys.Dispatch;

namespace Vixen.Data.Combinator {
	abstract public class Combinator<T, ResultType> : Dispatchable<T>, ICombinator<ResultType>, IAnyEvaluatorHandler
		where T : Combinator<T, ResultType>  {
		public void Combine(IEnumerable<IEvaluator> evaluators) {
			CombinatorValue = null;

			foreach(IEvaluator evaluator in evaluators) {
				evaluator.Dispatch(this);
			}
		}

		virtual public void Handle(IEvaluator<float> obj) { }

		virtual public void Handle(IEvaluator<DateTime> obj) { }

		virtual public void Handle(IEvaluator<Color> obj) { }

		virtual public void Handle(IEvaluator<long> obj) { }

		virtual public void Handle(IEvaluator<double> obj) { }

		virtual public void Handle(IEvaluator<LightingValue> obj) { }

		public ICommand<ResultType> CombinatorValue { get; protected set; }

		ICommand ICombinator.CombinatorValue {
			get { return CombinatorValue; }
		}
	}
}
