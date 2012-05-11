using System;
using Vixen.Commands;
using Vixen.Sys;

namespace Vixen.Data.Combinator {
	//CombinatorValue may be null because it's now a command instead of a value.
	public class PercentageHighestWinsCombinator : Combinator<PercentageHighestWinsCombinator, double> {
		override public void Handle(IEvaluator<double> obj) {
			if(CombinatorValue == null) {
				CombinatorValue = new DoubleValueCommand(obj.EvaluatorValue);
			} else {
				CombinatorValue.CommandValue = Math.Max(CombinatorValue.CommandValue, obj.EvaluatorValue);
			}
		}
	}
}
