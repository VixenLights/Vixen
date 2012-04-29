using System;
using Vixen.Commands;
using Vixen.Sys;

namespace Vixen.Data.Combinator {
	//CombinatorValue may be null because it's now a command instead of a value.
	public class FloatHighestWinsCombinator : Combinator<FloatHighestWinsCombinator, float> {
		override public void Handle(IEvaluator<float> obj) {
			if(CombinatorValue == null) {
				CombinatorValue = new FloatValue(obj.EvaluatorValue);
			} else {
				CombinatorValue.CommandValue = Math.Max(CombinatorValue.CommandValue, obj.EvaluatorValue);
			}
		}

		override public void Handle(IEvaluator<long> obj) {
			CombinatorValue.CommandValue = Math.Max(CombinatorValue.CommandValue, obj.EvaluatorValue);
		}

		// Double intentionally ignored.
		// Would turn the 0-1 double into a 0-1 float which would
		// cause it to be interpreted as an absolute value instead of
		// a % value.
	}
}
