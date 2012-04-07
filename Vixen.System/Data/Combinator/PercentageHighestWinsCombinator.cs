using System;
using Vixen.Sys;

namespace Vixen.Data.Combinator {
	public class PercentageHighestWinsCombinator : Combinator<PercentageHighestWinsCombinator, double> {
		override public void Handle(IEvaluator<double> obj) {
			Value = (float)Math.Max(Value, obj.Value);
		}
	}
	//public class PercentageHighestWinsCombinator : Dispatchable<PercentageHighestWinsCombinator>, ICombinator<double>, IAnyEvaluatorHandler {
	//    public void Combine(IEnumerable<IEvaluator> evaluators) {
	//        Value = 0;

	//        foreach(IEvaluator evaluator in evaluators) {
	//            evaluator.Dispatch(this);
	//        }
	//    }

	//    public void Handle(IEvaluator<float> obj) {
	//        // Ignored
	//    }

	//    public void Handle(IEvaluator<DateTime> obj) {
	//        // Ignored
	//    }

	//    public void Handle(IEvaluator<Color> obj) {
	//        // Ignored
	//    }

	//    public void Handle(IEvaluator<long> obj) {
	//        // Ignored
	//    }

	//    public void Handle(IEvaluator<double> obj) {
	//        Value = (float)Math.Max(Value, obj.Value);
	//    }

	//    public double Value { get; private set; }
	//}
}
