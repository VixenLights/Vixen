using System;
using System.Collections.Generic;
using System.Drawing;
using Vixen.Commands;
using Vixen.Sys;
using Vixen.Sys.Dispatch;

namespace Vixen.Data.Combinator {
	//*** Subclasses of this are currently ugly because every one of them has to branch on if CombinatorValue
	//    is null.  Is there another way?  It can only receive a single evaluator at a time due to
	//    double-dispatch and each value type needs to be handled separately and specifically to be able to
	//    create any meaningful or useful results.
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
